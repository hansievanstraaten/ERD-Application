using ERD.Git.Models;
using LibGit2Sharp;
using System.Diagnostics;

namespace ERD.Git
{
    /// <summary>
    /// Central Git interface for ERD projects.
    ///
    /// All Git operations used by ERD should go through this class.
    /// </summary>
    public static class ERDGit
    {
        #region REPOSITORY

        /// <summary>
        /// Determines whether the specified directory is a Git repository.
        /// </summary>
        public static bool IsRepository(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            return Repository.IsValid(path);
        }

        /// <summary>
        /// Finds the Git repository containing the specified directory.
        /// </summary>
        public static string? DiscoverRepository(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            return Repository.Discover(path);
        }

        /// <summary>
        /// Initializes a new Git repository.
        /// </summary>
        public static void Initialize(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Repository path is required.", nameof(path));

            Directory.CreateDirectory(path);

            if (!Repository.IsValid(path))
                Repository.Init(path);
        }

        /// <summary>
        /// Clones a remote repository.
        /// </summary>

        public static string Clone(GitCloneRequestModel request, bool checkout = true)
        {
            if (string.IsNullOrWhiteSpace(request.RemoteUrl))
                throw new ArgumentException(
                    "Remote URL is required.",
                    nameof(request.RemoteUrl));

            if (string.IsNullOrWhiteSpace(request.LocalPath))
                throw new ArgumentException(
                    "Local path is required.",
                    nameof(request.LocalPath));

            CloneOptions options = new CloneOptions
            {
                Checkout = checkout
            };

            if (!string.IsNullOrWhiteSpace(request.Username) &&
                !string.IsNullOrWhiteSpace(request.Password))
            {
                string username = request.Username;
                string password = request.Password;

                options.FetchOptions.CredentialsProvider =
                    (url, usernameFromUrl, types) =>
                    {
                        if (!types.HasFlag(SupportedCredentialTypes.UsernamePassword))
                            return null;

                        return new UsernamePasswordCredentials
                        {
                            Username = username,
                            Password = password
                        };
                    };
            }

            return Repository.Clone(
                request.RemoteUrl,
                request.LocalPath,
                options);
        }

        #endregion

        #region REMOTE / FETCH

        /// <summary>
        /// Gets all configured remotes.
        /// </summary>
        public static IReadOnlyList<GitRemoteInfo> GetRemotes(string repositoryPath)
        {
            using Repository repo = Open(repositoryPath);

            return repo.Network.Remotes
                .Select(r => new GitRemoteInfo
                {
                    Name = r.Name,
                    Url = r.Url,
                    PushUrl = r.PushUrl
                })
                .ToList();
        }

        public static string? GetCurrentBranchRemoteName(string repositoryPath)
        {
            using Repository repo = Open(repositoryPath);

            Branch branch = repo.Head;

            if (branch.TrackedBranch == null)
                return null;

            return branch.RemoteName;
        }

        //public static (string RemoteName, string UpstreamBranch)? GetCurrentBranchUpstream(string repositoryPath)
        //{
        //    using Repository repo = Open(repositoryPath);

        //    Branch branch = repo.Head;

        //    if (branch.TrackedBranch == null)
        //        return null;

        //    return (branch.RemoteName!, branch.TrackedBranch.CanonicalName);
        //}

        /// <summary>
        /// Adds a remote repository.
        /// </summary>
        public static void AddRemote(
            string repositoryPath,
            string name,
            string url)
        {
            using Repository repo = Open(repositoryPath);

            if (repo.Network.Remotes[name] != null)
                throw new InvalidOperationException($"Remote '{name}' already exists.");

            repo.Network.Remotes.Add(name, url);
        }

        /// <summary>
        /// Removes a configured remote.
        /// </summary>
        public static void RemoveRemote(
            string repositoryPath,
            string name)
        {
            using Repository repo = Open(repositoryPath);

            if (repo.Network.Remotes[name] == null)
                return;

            repo.Network.Remotes.Remove(name);
        }

        /// <summary>
        /// Fetches updates from a remote.
        ///
        /// This updates remote-tracking branches but does not modify
        /// the user's working branch.
        /// </summary>
        public static void Fetch(
            string repositoryPath,
            string remoteName,
            GitAuthentication authentication)
        {
            using Repository repo = Open(repositoryPath);

            Remote? remote = repo.Network.Remotes[remoteName];

            if (remote == null)
                throw new InvalidOperationException($"Remote '{remoteName}' does not exist.");

            List<string> refSpecs = remote.FetchRefSpecs
                .Select(x => x.Specification)
                .ToList();

            FetchOptions options = new FetchOptions();

            ApplyAuthentication(options, authentication);

            Commands.Fetch(repo, remote.Name, refSpecs, options, "ERD fetch");
        }

        public static int GetIncomingChangeCount(string repositoryPath)
        {
            using Repository repo = Open(repositoryPath);

            Branch branch = repo.Head;
            Branch? upstream = branch.TrackedBranch;

            if (upstream == null)
                throw new InvalidOperationException($"Branch '{branch.FriendlyName}' has no upstream.");

            return repo.Commits
                .QueryBy(new CommitFilter
                {
                    IncludeReachableFrom = upstream,
                    ExcludeReachableFrom = branch
                })
                .Count();
        }

        public static void OpenInBrowser(string repositoryPath, string remoteName)
        {
            using Repository repo = new Repository(repositoryPath);

            string remoteUrl = repo.Network.Remotes[remoteName]?.Url;

            if (!string.IsNullOrWhiteSpace(remoteUrl))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = remoteUrl,
                    UseShellExecute = true
                });
            }
        }

        #endregion

        #region BRANCHES

        /// <summary>
        /// Gets all local branches.
        /// </summary>
        public static IReadOnlyList<GitBranchInfo> GetLocalBranches(
            string repositoryPath)
        {
            using Repository repo = Open(repositoryPath);

            return repo.Branches
                .Where(b => !b.IsRemote)
                .Select(CreateBranchInfo)
                .ToList();
        }

        /// <summary>
        /// Gets all remote branches known locally.
        ///
        /// Call Fetch() first if you want the latest server state.
        /// </summary>
        public static IReadOnlyList<GitBranchInfo> GetRemoteBranches(
            string repositoryPath)
        {
            using Repository repo = Open(repositoryPath);

            return repo.Branches
                .Where(b => b.IsRemote)
                .Select(CreateBranchInfo)
                .ToList();
        }

        /// <summary>
        /// Creates a new local branch.
        /// </summary>
        public static void CreateBranch(
            string repositoryPath,
            string branchName,
            bool checkout = true)
        {
            using Repository repo = Open(repositoryPath);

            if (repo.Branches[branchName] != null)
                throw new InvalidOperationException($"Branch '{branchName}' already exists.");

            Branch? branch = repo.CreateBranch(branchName);

            if (checkout)
                Commands.Checkout(repo, branch);
        }

        /// <summary>
        /// Creates a local branch from a remote branch.
        /// </summary>
        public static void CreateBranchFromRemote(
            string repositoryPath,
            string remoteCanonicalBranchName,
            bool checkout = true)
        {
            using Repository repo = Open(repositoryPath);

            Branch? remoteBranch = repo.Branches[remoteCanonicalBranchName];

            if (remoteBranch == null)
                throw new InvalidOperationException($"Remote branch '{remoteCanonicalBranchName}' does not exist.");

            if (!remoteBranch.IsRemote)
                throw new InvalidOperationException($"'{remoteCanonicalBranchName}' is not a remote branch.");

            //GitBranchInfo branchInfo = remoteBranches.FirstOrDefault(b => b.CanonicalName == remotes.BranchName);
            string remoteName = $"{remoteBranch.RemoteName}/";
            int startIndex = remoteCanonicalBranchName.IndexOf(remoteName) + remoteName.Length;
            string localBranchName = remoteCanonicalBranchName.Substring(startIndex);         

            Branch? localBranch = repo.CreateBranch(localBranchName, remoteBranch.Tip);

            repo.Branches.Update(localBranch, b => b.TrackedBranch = remoteBranch.CanonicalName);

            if (checkout)
                Commands.Checkout(repo, localBranch);
        }

        /// <summary>
        /// Checks out a branch.
        /// </summary>
        public static void Checkout(
            string repositoryPath,
            string branchName)
        {
            using Repository repo = Open(repositoryPath);

            Branch? branch = repo.Branches[branchName];

            if (branch == null)
                throw new InvalidOperationException($"Branch '{branchName}' does not exist.");

            Commands.Checkout(repo, branch);
        }

        /// <summary>
        /// Deletes a local branch.
        /// </summary>
        public static void DeleteBranch(
            string repositoryPath,
            string branchName,
            bool force = false)
        {
            using Repository repo = Open(repositoryPath);

            Branch? branch = repo.Branches[branchName];

            if (branch == null)
                return;

            if (repo.Head.FriendlyName == branchName)
                throw new InvalidOperationException("Cannot delete the currently checked out branch.");

            repo.Branches.Remove(branch);
        }

        /// <summary>
        /// Gets the currently checked out branch.
        /// </summary>
        public static GitBranchInfo? GetCurrentBranch(
            string repositoryPath)
        {
            using Repository repo = Open(repositoryPath);

            if (repo.Head == null)
                return null;

            return CreateBranchInfo(repo.Head);
        }

        #endregion

        #region PULL

        /// <summary>
        /// Pulls the current branch from its configured upstream branch.
        /// </summary>
        public static PullResult Pull(
            string repositoryPath,
            GitIdentity identity,
            GitAuthentication authentication)
        {
            using Repository repo = Open(repositoryPath);

            Signature signature = CreateSignature(identity);

            PullOptions options = new PullOptions
            {
                FetchOptions = new FetchOptions(),
                MergeOptions = new MergeOptions
                {
                    CommitOnSuccess = true
                }
            };

            ApplyAuthentication(options.FetchOptions,
                authentication);

            MergeResult result = Commands.Pull(
                repo,
                signature,
                options);

            List<string?> conflictedFiles = repo.Index.Conflicts
                .Select(conflict => conflict.Ours?.Path
                    ?? conflict.Theirs?.Path
                    ?? conflict.Ancestor?.Path)
                .Where(path => path != null)
                .Distinct()
                .ToList();

            return new PullResult
            {
                CommitId = result.Commit?.Sha,
                Status = result.Status.ToString(),
                ConflictedFiles = conflictedFiles
            };
        }

        #endregion

        #region PUSH

        /// <summary>
        /// Pushes the current branch to its configured upstream.
        /// </summary>
        public static void Push(string repositoryPath,
            string? remoteName,
            GitAuthentication? authentication)
        {
            using Repository repo = Open(repositoryPath);

            Branch branch = repo.Head;

            Remote[] remotes = repo.Network.Remotes.ToArray();

            Remote? remote = branch.TrackedBranch != null
                ? repo.Network.Remotes[branch.RemoteName]
                : remoteName != null
                    ? repo.Network.Remotes[remoteName]
                    : remotes.Length == 1
                        ? remotes[0]
                        : null;

            if (remote == null)
            {
                throw new InvalidOperationException(
                    remotes.Length == 0
                        ? "The repository has no configured remotes."
                        : "Cannot determine the target remote. Please select one.");
            }

            PushOptions options = new PushOptions();
            ApplyAuthentication(options, authentication);
            //origin => https://github.com/hansievanstraaten/Reports_DB_ERD.git

            // Push the local branch to a branch of the same name.
            repo.Network.Push(
                remote,
                branch.CanonicalName,
                $"refs/heads/{branch.FriendlyName}",
                options);

            // Configure upstream after a successful push.
            repo.Branches.Update(
                branch,
                b => b.Remote = remote.Name,
                b => b.UpstreamBranch = $"refs/heads/{branch.FriendlyName}");
        }

        /// <summary>
        /// Pushes a branch to a remote and optionally establishes
        /// the upstream relationship.
        /// </summary>
        public static void PushBranch(
            string repositoryPath,
            string branchName,
            string remoteName,
            bool setUpstream = true,
            GitAuthentication? authentication = null)
        {
            using Repository repo = Open(repositoryPath);

            Branch? branch = repo.Branches[branchName];

            if (branch == null)
                throw new InvalidOperationException($"Branch '{branchName}' does not exist.");

            Remote? remote = repo.Network.Remotes[remoteName];

            if (remote == null)
                throw new InvalidOperationException($"Remote '{remoteName}' does not exist.");

            PushOptions options = new PushOptions();

            ApplyAuthentication(options, authentication);

            string refSpec = $"refs/heads/{branchName}:refs/heads/{branchName}";

            repo.Network.Push(
                remote,
                refSpec,
                options);

            if (setUpstream)
            {
                Branch? remoteBranch = repo.Branches[$"{remoteName}/{branchName}"];

                if (remoteBranch != null)
                {
                    repo.Branches.Update(branch, b => b.TrackedBranch = remoteBranch.CanonicalName);
                }
            }
        }

        #endregion

        #region COMMIT

        /// <summary>
        /// Stages all changes in the repository.
        /// </summary>
        public static void StageAll(string repositoryPath)
        {
            using Repository repo = Open(repositoryPath);

            Commands.Stage(repo, "*");
        }

        /// <summary>
        /// Stages a specific file.
        /// </summary>
        public static void Stage(
            string repositoryPath,
            string relativePath)
        {
            using Repository repo = Open(repositoryPath);

            Commands.Stage(repo, relativePath);
        }

        /// <summary>
        /// Unstages a file.
        /// </summary>
        public static void Unstage(
            string repositoryPath,
            string relativePath)
        {
            using Repository repo = Open(repositoryPath);

            Commands.Unstage(repo, relativePath);
        }

        /// <summary>
        /// Commits all currently staged changes.
        /// </summary>
        public static string Commit(
            string repositoryPath,
            GitCommitRequest request)
        {
            using Repository repo = Open(repositoryPath);

            Signature signature = CreateSignature((GitIdentity)request);

            Commit commit = repo.Commit(request.CommitMessage, signature, signature);

            return commit.Sha;
        }

        /// <summary>
        /// Stages all changes and commits them.
        /// </summary>
        public static string StageAndCommit(
            string repositoryPath,
            GitCommitRequest request)
        {
            StageAll(repositoryPath);

            return Commit(repositoryPath, request);
        }

        #endregion

        #region STATUS

        /// <summary>
        /// Gets the current working tree status.
        /// </summary>
        public static IReadOnlyList<GitFileStatus> GetStatus(
            string repositoryPath)
        {
            using Repository repo = Open(repositoryPath);

            return repo.RetrieveStatus()
                .Select(x => new GitFileStatus
                {
                    Path = x.FilePath,
                    Status = x.State.ToString()
                })
                .ToList();
        }

        /// <summary>
        /// Determines whether the repository contains uncommitted changes.
        /// </summary>
        public static bool HasChanges(string repositoryPath)
        {
            using Repository repo = Open(repositoryPath);

            return repo.RetrieveStatus().Any();
        }

        public static List<string?> GetConflicts(string repositoryPath)
        {
            using Repository repo = Open(repositoryPath);

            List<string?> conflictedFiles = repo.Index.Conflicts
                .Select(conflict => conflict.Ours?.Path
                    ?? conflict.Theirs?.Path
                    ?? conflict.Ancestor?.Path)
                .Where(path => path != null)
                .Distinct()
                .ToList();

            return conflictedFiles;//.Where(path => path != null).Select(path => path!).ToList();
        }

        #endregion

        #region STASH

        /// <summary>
        /// Stashes the current working changes.
        /// </summary>
        public static void Stash(
            string repositoryPath,
            GitIdentity identity,
            string? message = null,
            bool includeUntracked = true)
        {
            using Repository repo = Open(repositoryPath);

            Signature signature = CreateSignature(identity);

            StashModifiers options = new StashModifiers();

            if (includeUntracked)
                options |= StashModifiers.IncludeUntracked;

            repo.Stashes.Add(
                signature,
                message,
                options);
        }

        /// <summary>
        /// Gets all stashes.
        /// </summary>
        public static IReadOnlyList<GitStashInfo> GetStashes(
            string repositoryPath)
        {
            using Repository repo = Open(repositoryPath);

            return repo.Stashes
                .Select((stash, index) => new GitStashInfo
                {
                    Index = index,
                    Message = stash.Message
                })
                .ToList();
        }

        /// <summary>
        /// Applies a stash without deleting it.
        /// </summary>
        public static void ApplyStash(
            string repositoryPath,
            int stashIndex = 0)
        {
            using Repository repo = Open(repositoryPath);

            repo.Stashes.Apply(stashIndex);
        }

        /// <summary>
        /// Applies a stash and removes it from the stash list.
        /// </summary>
        public static void PopStash(
            string repositoryPath,
            int stashIndex = 0)
        {
            using Repository repo = Open(repositoryPath);

            repo.Stashes.Pop(stashIndex);
        }

        /// <summary>
        /// Removes a stash without applying it.
        /// </summary>
        public static void DropStash(
            string repositoryPath,
            int stashIndex = 0)
        {
            using Repository repo = Open(repositoryPath);

            repo.Stashes.Remove(stashIndex);
        }

        #endregion

        #region HISTORY

        /// <summary>
        /// Gets commit history for the current branch.
        /// </summary>
        public static IReadOnlyList<GitCommitInfo> GetHistory(
            string repositoryPath,
            int maxCount = 100)
        {
            using Repository repo = Open(repositoryPath);

            return repo.Commits
                .Take(maxCount)
                .Select(c => new GitCommitInfo
                {
                    Sha = c.Sha,
                    ShortSha = c.Sha[..Math.Min(7, c.Sha.Length)],
                    Message = c.MessageShort,
                    Author = c.Author.Name,
                    AuthorEmail = c.Author.Email,
                    Date = c.Author.When
                })
                .ToList();
        }

        #endregion

        #region TAGS

        /// <summary>
        /// Gets all tags.
        /// </summary>
        public static IReadOnlyList<string> GetTags(
            string repositoryPath)
        {
            using Repository repo = Open(repositoryPath);

            return repo.Tags
                .Select(t => t.FriendlyName)
                .OrderBy(x => x)
                .ToList();
        }

        /// <summary>
        /// Creates a lightweight tag.
        /// </summary>
        public static void CreateTag(
            string repositoryPath,
            string tagName,
            string? targetCommitSha = null)
        {
            using Repository repo = Open(repositoryPath);

            Commit? commit =
                targetCommitSha == null
                    ? repo.Head.Tip
                    : repo.Lookup<Commit>(targetCommitSha);

            if (commit == null)
                throw new InvalidOperationException("Target commit could not be found.");

            repo.ApplyTag(
                tagName,
                commit.Sha);
        }

        #endregion

        #region RESET / DISCARD

        /// <summary>
        /// Resets the working tree.
        /// </summary>
        public static void Reset(
            string repositoryPath,
            ResetMode mode = ResetMode.Mixed)
        {
            using Repository repo = Open(repositoryPath);

            repo.Reset(mode);
        }

        /// <summary>
        /// Discards all uncommitted changes.
        ///
        /// WARNING: This permanently discards changes.
        /// </summary>
        public static void DiscardChanges(
            string repositoryPath)
        {
            using Repository repo = Open(repositoryPath);

            Commands.Stage(repo, "*");

            RepositoryStatus status = repo.RetrieveStatus();

            foreach (StatusEntry? item in status)
            {
                Commands.Unstage(repo, item.FilePath);
            }

            Commands.Checkout(
                repo,
                repo.Head.Tip,
                new CheckoutOptions
                {
                    CheckoutModifiers =
                        CheckoutModifiers.Force
                });
        }

        #endregion

        #region REPOSITORY INFORMATION

        /// <summary>
        /// Gets general information about the repository.
        /// </summary>
        public static GitRepositoryInfo GetRepositoryInfo(
            string repositoryPath)
        {
            using Repository repo = Open(repositoryPath);

            Branch? head = repo.Head;

            return new GitRepositoryInfo
            {
                Path = repo.Info.WorkingDirectory,
                CurrentBranch = head?.FriendlyName,
                HeadCommit = head?.Tip?.Sha,
                IsDirty = repo.RetrieveStatus().Any(),
                IsBare = repo.Info.IsBare
            };
        }

        #endregion

        #region GIT IGNORE

        /// <summary>
        /// Adds an entry to .gitignore.
        /// </summary>
        public static void AddGitIgnore(
            string repositoryPath,
            string pattern)
        {
            string gitIgnorePath =
                Path.Combine(repositoryPath, ".gitignore");

            List<string> lines =
                File.Exists(gitIgnorePath)
                    ? File.ReadAllLines(gitIgnorePath).ToList()
                    : new List<string>();

            if (!lines.Contains(pattern))
            {
                lines.Add(pattern);
                File.WriteAllLines(gitIgnorePath, lines);
            }
        }

        #endregion

        #region INTERNAL HELPERS

        private static Repository Open(string repositoryPath)
        {
            if (string.IsNullOrWhiteSpace(repositoryPath))
                throw new ArgumentException("Repository path is required.", nameof(repositoryPath));

            if (!Repository.IsValid(repositoryPath))
                throw new InvalidOperationException($"'{repositoryPath}' is not a Git repository.");

            return new Repository(repositoryPath);
        }

        private static Signature CreateSignature(
            GitIdentity identity)
        {
            return new Signature(
                identity.Name,
                identity.Email,
                DateTimeOffset.Now);
        }

        //private static Signature CreateSignature()
        //{
        //    using WindowsIdentity identity = WindowsIdentity.GetCurrent();


        //    return new Signature(
        //        identity.Name,
        //        string.Empty,
        //        DateTimeOffset.Now);
        //}

        private static GitBranchInfo CreateBranchInfo(
            Branch branch)
        {
            return new GitBranchInfo
            {
                Name = branch.FriendlyName,
                CanonicalName = branch.CanonicalName,
                RemoteName = branch.RemoteName,
                IsRemote = branch.IsRemote,
                IsCurrent = branch.IsCurrentRepositoryHead,
                IsTracking = branch.TrackedBranch != null,
                TrackedBranch =
                    branch.TrackedBranch?.FriendlyName,
                CommitSha = branch.Tip?.Sha,
                AheadBy = branch.TrackingDetails?.AheadBy,
                BehindBy = branch.TrackingDetails?.BehindBy
            };
        }

        private static void ApplyAuthentication(
            FetchOptions options,
            GitAuthentication? authentication)
        {
            if (authentication == null
             || string.IsNullOrWhiteSpace(authentication.Password))
                return;

            options.CredentialsProvider =
                (_, _, _) =>
                    new UsernamePasswordCredentials
                    {
                        Username = authentication.Username,
                        Password = authentication.Password
                    };
        }

        private static void ApplyAuthentication(
            PushOptions options,
            GitAuthentication? authentication)
        {
            if (authentication == null
             || string.IsNullOrWhiteSpace(authentication.Password))
                return;

            options.CredentialsProvider =
                (_, _, _) =>
                    new UsernamePasswordCredentials
                    {
                        Username = authentication.Username,
                        Password = authentication.Password
                    };
        }

        private static void ApplyAuthentication(
            CloneOptions options,
            GitAuthentication? authentication)
        {
            ApplyAuthentication(
                options.FetchOptions,
                authentication);
        }

        public static void CreateLocalFromRemoteSelection(string repoPath)
        {
            using (var repo = new Repository(repoPath))
            {
                // 1. Get all remote-tracking branches dynamically
                var remoteBranches = repo.Branches
                    .Where(b => b.IsRemote)
                    .ToList();

                if (!remoteBranches.Any())
                {
                    Console.WriteLine("No remote branches found. Please fetch from your remote first.");
                    return;
                }

                // 2. Display the list cleanly to the user
                Console.WriteLine("Available Remote Branches:");
                for (int i = 0; i < remoteBranches.Count; i++)
                {
                    // FriendlyName returns "origin/feature-branch"
                    // RemoteName returns "origin" safely, regardless of the cloud host
                    Console.WriteLine($"[{i}] {remoteBranches[i].FriendlyName} (Remote: {remoteBranches[i].RemoteName})");
                }

                // 3. Capture user choice
                Console.Write("\nSelect a branch number to checkout: ");
                if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice >= remoteBranches.Count)
                {
                    Console.WriteLine("Invalid selection.");
                    return;
                }

                Branch selectedRemoteBranch = remoteBranches[choice];

                // 4. Safely extract the raw branch name without the remote prefix
                // LibGit2Sharp natively calculates the upstream destination via its Canonical Name
                string cleanBranchName = selectedRemoteBranch.FriendlyName
                    .Substring(selectedRemoteBranch.RemoteName.Length + 1); // Trims "origin/"

                // 5. Check if the local branch already exists
                Branch localBranch = repo.Branches[cleanBranchName];

                if (localBranch == null)
                {
                    Console.WriteLine($"Creating local branch '{cleanBranchName}' tracking '{selectedRemoteBranch.FriendlyName}'...");

                    // Point the local branch head directly to the exact commit reference of the remote
                    localBranch = repo.CreateBranch(cleanBranchName, selectedRemoteBranch.Tip);

                    // Set up standard tracking configuration upstream
                    repo.Branches.Update(localBranch, b => b.TrackedBranch = selectedRemoteBranch.CanonicalName);
                }
                else
                {
                    Console.WriteLine($"Local branch '{cleanBranchName}' already exists. Skipping creation.");
                }

                // 6. Checkout the local branch
                Commands.Checkout(repo, localBranch);
                Console.WriteLine($"Successfully switched to branch: {repo.Head.FriendlyName}");
            }
        }

        #endregion
    }
}