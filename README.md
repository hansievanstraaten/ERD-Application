<html>
	<head>
<meta name="google-site-verification" content="38kYglhbxjlpieXktL1PYbHf0OWmXSAeWUHgsBjtmn0" />
<meta name="msvalidate.01" content="38F9DFAC225565BA82DE475535B2BC6A" />
</head>
	<body>
	</body>
</html>

Welcome to ViSo-Nice ERD Application.<BR/><BR/>

<a style="color:#0000FF" href="https://raw.githubusercontent.com/hansievanstraaten/ERD-Application/master/ERD%20Msi/ViSo.Viewer.msi" download="ViSo.Viewer.msi">Download Application</a>


The Application was developed as part of a necessity to have a visual representation on a database and managing database entities as part of the representation.

Support for multiple databases, without the necessity to create a new instance of the ERD Model. Alternative database connections are not required to provide a password that makes this application ideal for deployment to production environments where the production database is not available for development.

Multiple canvases per model allows for smaller canvases and acts as one great ERD. Each canvas is saved on its own that allows for developers to work on the same ERD Model without Merage conflicts. For Later development the locking of canvases will be implemented to ensure that developers do not work on the same canvas at the same time.
Files are saved in Json format that provides editability in raw file format, for those who are brave, though I cannot guarantee correct application functionality one a file has been edited.

Only table that are on a canvas are active, and are updated during Reverse and Forward Engineering, as well as File Scripting.

File Scripting is available for C#, but I would love to add more languages, if you would like a language implementation to feel free to ask or download the source and add the functionality yourself. I would however like if you share your changes once it is tested and correct. Scripting is a bit tricky and I will provide a document as soon as posable on this matter.
Current development only supports MS SQL, and as with the scripting languages, I would like to extend this, though this will be a bit trickier and some specialist assistance in the database arena would be appreciated.

Virtual relations allow for relations to be created without any database constraint. These types of relations are only visible on the canvas and will not be deployed to your database. Thus, you can create relations on your ERD and don’t need to worry about database performance on larger database models where data IO is of more importance than relational management.

The Application are ideal for small to larger size databases, and I tested it on a database consisting of about 900 tables. Tends to be a bit slow on some functions such as database comparisons, but in general performance was good.

This desktop application is developed in C# using WPF, .Net Framework 4.7.
Upgrades and changes will be periodic, and without notice. 
The Msi Build will change after each stable version.

Happy Huntings,
Hope you can use this application to your own benefit.
