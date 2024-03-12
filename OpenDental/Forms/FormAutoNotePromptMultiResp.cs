using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormAutoNotePromptMultiResp:FormODBase {
		///<summary>Set this value externally.</summary>
		public string PromptText;
		///<summary>What the user picked.</summary>
		public string ResultText;
		///<summary>The string value representing the list to pick from.  One item per line.</summary>
		public string PromptOptions;
		///<summary>If user has the option to go back</summary>
		public bool IsGoBack;
		///<summary>The string value of previous user response</summary>
		public string PromptResponseCur;
		/// <summary>The string of the autonote description which is used to bring up the actual prompt.</summary>
		private string _noteDescription;
		///<summary>Characters used to indicated the separation between two or more response selections when concatenated into a single string.</summary>
		private const string DELIMITER=", ";

		public FormAutoNotePromptMultiResp(string noteDescription) {
			InitializeComponent();
			InitializeLayoutManager();
			_noteDescription=noteDescription;
			Lan.F(this);
		}

		private void FormAutoNotePromptMultiResp_Load(object sender,EventArgs e) {
			PromptResponseCur=!string.IsNullOrEmpty(PromptResponseCur) ? PromptResponseCur : "";
			Location=new Point(Left,Top+150);
			labelPrompt.Text=PromptText;
			butBack.Visible=IsGoBack;
			string[] stringArrayLines=PromptOptions.Split(new string[] {"\r\n"},StringSplitOptions.RemoveEmptyEntries);
			string[] stringArrayResponseLines=PromptResponseCur.Split(new string[] {DELIMITER},StringSplitOptions.RemoveEmptyEntries);
			for(int i=0;i<stringArrayLines.Length;i++) {
				listMain.Items.Add(stringArrayLines[i]);
				if(stringArrayResponseLines.Contains(stringArrayLines[i])) {//display previous user reponses
					listMain.SetItemChecked(listMain.Items.Count-1,true);
				}
			}
			if(PrefC.GetBool(PrefName.ProcNoteSigsBlockedAutoNoteIncomplete)) {
				butSkip.Enabled=false;
			}
		}
		
		private void butSelectAll_Click(object sender,EventArgs e) {
			for(int i=0;i<listMain.Items.Count;i++) {
				listMain.SetItemChecked(i,true);
			}
		}

		private void butSelectNone_Click(object sender,EventArgs e) {
			for(int i=0;i<listMain.Items.Count;i++) {
				listMain.SetItemChecked(i,false);
			}
		}

		private void FormAutoNotePromptMultiResp_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode==Keys.Enter) {
				AdvanceToNextAutoNote();
			}
		}

		private void AdvanceToNextAutoNote() {
			ResultText="";
			for(int i=0;i<listMain.CheckedIndices.Count;i++) {
				if(i>0) {
					ResultText+=DELIMITER;
				}
				ResultText+=listMain.CheckedItems[i].ToString();
			}
			DialogResult=DialogResult.OK;
		}

		private void butRemovePrompt_Click(object sender,EventArgs e) {
			ResultText="";
			DialogResult=DialogResult.OK;
		}

		private void butNext_Click(object sender,EventArgs e) {
			if(listMain.CheckedIndices.Count>0) {
				AdvanceToNextAutoNote();
				return;
			}
			ResultText="[Prompt:\""+_noteDescription+"\"]";
			DialogResult=DialogResult.OK;
		}

		private void butBack_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Retry;
			Close();
		}

		private void butPreview_Click(object sender,EventArgs e) {
			ResultText="";
			for(int i=0;i<listMain.CheckedIndices.Count;i++) {
				if(i>0) {
					ResultText+=DELIMITER;
				}
				ResultText+=listMain.CheckedItems[i].ToString();
			}
			using FormAutoNotePromptPreview formAutoNotePromptPreview=new FormAutoNotePromptPreview();
			formAutoNotePromptPreview.ResultText=ResultText;
			formAutoNotePromptPreview.ShowDialog();
			if(formAutoNotePromptPreview.DialogResult==DialogResult.OK) {
				ResultText=formAutoNotePromptPreview.ResultText;
				DialogResult=DialogResult.OK;
			}
		}

		private void butExit_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Abort autonote entry?")) {
				return;
			}
			DialogResult=DialogResult.Cancel;
		}

		
	}
}