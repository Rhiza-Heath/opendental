using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
///<summary></summary>
	public partial class FrmInsPlanSelect:FrmODBase {
		///<summary>After closing this form, this will contain the selected Plan.</summary>
		public InsPlan InsPlanSelected;
		///<summary>Supply a string here to start off the search with filtered employers.</summary>
		public string empText;
		///<summary>Supply a string here to start off the search with filtered carriers.</summary>
		public string carrierText;
		private DataTable _table;
		private bool trojan;
		///<summary>Supply a string here to start off the search with filtered group names.</summary>
		public string groupNameText;
		///<summary>Supply a string here to start off the search with filtered group nums.</summary>
		public string groupNumText;

		///<summary></summary>
		public FrmInsPlanSelect(){
			InitializeComponent();// Required for Windows Form Designer support
			Lang.F(this);
			Load+=FrmInsPlanSelect_Load;
			gridMain.CellDoubleClick+=gridMain_CellDoubleClick;
			textEmployer.TextChanged+=textEmployer_TextChanged;
			textCarrier.TextChanged+=textCarrier_TextChanged;
			textGroupName.TextChanged+=textGroupName_TextChanged;
			textGroupNum.TextChanged+=textGroupNum_TextChanged;
			textPlanNum.TextChanged+=textPlanNum_TextChanged;
			textTrojanID.TextChanged+=textTrojanID_TextChanged;
			radioOrderCarrier.Click+=radioOrderCarrier_Click;
			radioOrderEmp.Click+=radioOrderEmp_Click;
			checkShowHidden.Click+=CheckShowHidden_Click;
			PreviewKeyDown+=FrmInsPlanSelect_PreviewKeyDown;
		}

		private void FrmInsPlanSelect_Load(object sender, System.EventArgs e) {
			//USA uses "Group Num", Canada uses "Plan Number"
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				labelGroupNum.Text=Lang.g(this,"Plan Number");
			}
			else {
				labelGroupNum.Text=Lang.g(this,"Group Num");
			}
			Program program=Programs.GetCur(ProgramName.Trojan);
			if(program!=null && program.Enabled) {
				trojan=true;
			}
			else{
				labelTrojanID.Visible=false;
				textTrojanID.Visible=false;
			}
			textEmployer.Text=empText;
			textCarrier.Text=carrierText;
			textGroupName.Text=groupNameText;
			textGroupNum.Text=groupNumText;
			FillGrid();
		}

		private void FillGrid(bool isGetAll=false){
			Cursor=Cursors.Wait;
			_table=InsPlans.GetBigList(radioOrderEmp.Checked,textEmployer.Text,textCarrier.Text,
				textGroupName.Text,textGroupNum.Text,textPlanNum.Text,textTrojanID.Text,checkShowHidden.Checked==true,isGetAll);
			butBlank.IsEnabled=Security.IsAuthorized(EnumPermType.InsPlanEdit,true);
			int selectedRow;//preserves the selected row.
			if(gridMain.SelectedIndices.Length==1){
				selectedRow=gridMain.SelectedIndices[0];
			}
			else{
				selectedRow=-1;
			}
			//USA uses Group Num, Canada uses Plan Num
			string groupNum="Group#";
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				groupNum="Plan#";
			}
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lans.g("TableInsurancePlans","Employer"),140);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lans.g("TableInsurancePlans","Carrier"),140);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lans.g("TableInsurancePlans","Phone"),82);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lans.g("TableInsurancePlans","Address"),120);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lans.g("TableInsurancePlans","City"),80);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lans.g("TableInsurancePlans","ST"),25);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lans.g("TableInsurancePlans","Zip"),50);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lans.g("TableInsurancePlans",groupNum),70);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lans.g("TableInsurancePlans","Group Name"),90);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lans.g("TableInsurancePlans","noE"),35);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lans.g("TableInsurancePlans","ElectID"),45);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lans.g("TableInsurancePlans","Subs"),40);
			gridMain.Columns.Add(col);
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				col=new GridColumn(Lans.g("TableCarriers","CDAnet"),50);
				gridMain.Columns.Add(col);
			}
			if(trojan){
				col=new GridColumn(Lans.g("TableInsurancePlans","TrojanID"),60);
				gridMain.Columns.Add(col);
			}
			//PlanNote not shown
			gridMain.ListGridRows.Clear();
			GridRow row;
			//Carrier carrier;
			for(int i=0;i<_table.Rows.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_table.Rows[i]["EmpName"].ToString());
				row.Cells.Add(_table.Rows[i]["CarrierName"].ToString());
				row.Cells.Add(_table.Rows[i]["Phone"].ToString());
				row.Cells.Add(_table.Rows[i]["Address"].ToString());
				row.Cells.Add(_table.Rows[i]["City"].ToString());
				row.Cells.Add(_table.Rows[i]["State"].ToString());
				row.Cells.Add(_table.Rows[i]["Zip"].ToString());
				row.Cells.Add(_table.Rows[i]["GroupNum"].ToString());
				row.Cells.Add(_table.Rows[i]["GroupName"].ToString());
				row.Cells.Add(_table.Rows[i]["noSendElect"].ToString());
				row.Cells.Add(_table.Rows[i]["ElectID"].ToString());
				row.Cells.Add(_table.Rows[i]["subscribers"].ToString());
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
					row.Cells.Add((_table.Rows[i]["IsCDA"].ToString()=="0")?"":"X");
				}
				if(trojan){
					row.Cells.Add(_table.Rows[i]["TrojanID"].ToString());
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			gridMain.SetSelected(selectedRow,true);
			Cursor=Cursors.Arrow;
		}

		private bool InsPlanExists(InsPlan insPlan) {
			if(insPlan==null || insPlan.PlanNum==0) {
				MsgBox.Show(this,"Insurance plan selected no longer exists.");
				FillGrid();
				return false;
			}
			return true;
		}

		private void gridMain_CellDoubleClick(object sender,GridClickEventArgs e){
			InsPlan insPlan=InsPlans.GetPlan(PIn.Long(_table.Rows[e.Row]["PlanNum"].ToString()),null);
			if(!InsPlanExists(insPlan)) {
				return;
			}
			InsPlanSelected=insPlan.Copy();
			IsDialogOK=true;
		}

		private void radioOrderEmp_Click(object sender, System.EventArgs e) {
			FillGrid();
		}

		private void radioOrderCarrier_Click(object sender, System.EventArgs e) {
			FillGrid();
		}

		private void textEmployer_TextChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void textCarrier_TextChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void textGroupName_TextChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void textGroupNum_TextChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void textPlanNum_TextChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void textTrojanID_TextChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void CheckShowHidden_Click(object sender,EventArgs e) {
			FillGrid();
		}
		
		private void butGetAll_Click(object sender,EventArgs e) {
			FillGrid(true);
		}

		private void butBlank_Click(object sender, System.EventArgs e) {
			InsPlanSelected=new InsPlan();
			IsDialogOK=true;
		}

		private void FrmInsPlanSelect_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butOK.IsAltKey(Key.O,e)) {
				butOK_Click(this,new EventArgs());
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(gridMain.SelectedIndices.Length==0){
				MessageBox.Show(Lans.g(this,"Please select an item first."));
				return;
			}
			if(gridMain.SelectedIndices.Length>1) {
				MessageBox.Show(Lans.g(this,"Please select only one item first."));
				return;
			}
			InsPlan insPlan=InsPlans.GetPlan(PIn.Long(_table.Rows[gridMain.SelectedIndices[0]]["PlanNum"].ToString()),null);
			if(!InsPlanExists(insPlan)) {
				return;
			}
			InsPlanSelected=insPlan;
			IsDialogOK=true;
		}

	}
}