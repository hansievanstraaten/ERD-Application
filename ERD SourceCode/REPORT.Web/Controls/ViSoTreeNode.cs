using GeneralExtensions;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace REPORT.Web.Controls
{
	public class ViSoTreeNode : TreeNode
	{
		//public ViSoTreeNode(string text) : base(text)
		//{
		//}

		public ViSoTreeNode(long carierId, string text) : base($"<span class='ViSo-Text' CarierId='{carierId}'  >{text}</span>")
		{
		}

		public ViSoTreeNode(TreeNode node) : base(node.Text)
		{
			string attributeKey = "CarierId='";
				
			int attributeIndex = node.Text.IndexOf(attributeKey) + attributeKey.Length;

			if (attributeIndex > 0)
			{
				StringBuilder carierText = new StringBuilder();

				while(node.Text.Substring(attributeIndex, 1) != "'")
				{
					carierText.Append(node.Text.Substring(attributeIndex, 1));

					++attributeIndex;
				}

				this.CarierId = carierText.ToString().TryToInt64();
			}
		}

		public long CarierId { get; set; }


	}
}