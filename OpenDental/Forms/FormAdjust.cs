using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using System.Linq;
using CodeBase;

namespace OpenDental {
	///<summary></summary>
	public partial class FormAdjust : FormODBase {
		///<summary></summary>
		public bool IsNew;
		private Patient _patient;
		private Adjustment _adjustment;
		///<summary>When true, the OK click will not let the user leave the window unless the check amount is 0.</summary>
		private bool _checkZeroAmount;
		///<summary>All positive adjustment defs.</summary>
		private List<Def> _listDefsAdjPosCats;
		///<summary>All negative adjustment defs.</summary>
		private List<Def> _listDefsAdjNegCats;
		///<summary>Filtered list of providers based on which clinic is selected. If no clinic is selected displays all providers. Also includes a dummy clinic at index 0 for "none"</summary>
		//private List<Provider> _listProviders;
		private decimal _adjRemAmt;
		private bool _isEditAnyway;
		private List<PaySplit> _listPaySplitsForAdjustment;
		private bool _isNegativeAdjustment;
		private List<long> _listTsiExcludedAdjDefNums;
		private Program _program;
		private Patient _patientGuar;
		private bool _isTsiAdj;

		///<summary></summary>
		public FormAdjust(Patient patient,Adjustment adjustment,bool isTsiAdj=false){
			InitializeComponent();
			InitializeLayoutManager();
			_patient=patient;
			_adjustment=adjustment;
			_program=Programs.GetCur(ProgramName.Transworld);
			_patientGuar=Patients.GetGuarForPat(_adjustment.PatNum);
			_isTsiAdj=isTsiAdj;
			Lan.F(this);
		}

		private void FormAdjust_Load(object sender, System.EventArgs e) {
			if(AvaTax.IsEnabled()) {
				//We do not want to allow the user to make edits or delete SalesTax and SalesTaxReturn Adjustments.  Popup if no permission so user knows why disabled.
				if(AvaTax.IsEnabled() && 
					(_adjustment.AdjType==AvaTax.GetSalesTaxAdjType() || _adjustment.AdjType==AvaTax.GetSalesTaxReturnAdjType()) && 
					!Security.IsAuthorized(EnumPermType.SalesTaxAdjEdit)) {
					DisableAllExcept(textNote);
					textNote.ReadOnly=true;//This will allow the user to copy the note if desired.
				}
			}
			if(IsNew){
				if(!Security.IsAuthorized(EnumPermType.AdjustmentCreate,DateTime.Now,true)) {//Date not checked here.  Message will show later.
					if(!Security.IsAuthorized(EnumPermType.AdjustmentEditZero,true)) {//Let user create an adjustment of zero if they have this perm.
						MessageBox.Show(Lans.g("Security","Not authorized for")+"\r\n"+GroupPermissions.GetDesc(EnumPermType.AdjustmentCreate));
						DialogResult=DialogResult.Cancel;
						return;
					}
					//Make sure amount is 0 after OK click.
					_checkZeroAmount=true;
				}
			}
			else{
				Def def=Defs.GetDef(DefCat.AdjTypes,_adjustment.AdjType);
				if(!GroupPermissions.HasPermissionForAdjType(EnumPermType.AdjustmentEdit,def,_adjustment.AdjDate,suppressMessage:false)){
					butOK.Enabled=false;
					butDelete.Enabled=false;
					//User can't edit but has edit zero amount perm.  Allow delete only if date is today.
					if(GroupPermissions.HasPermissionForAdjType(EnumPermType.AdjustmentEditZero,def) 
						&& _adjustment.AdjAmt==0
						&& _adjustment.DateEntry.Date==MiscData.GetNowDateTime().Date) 
					{
						butDelete.Enabled=true;
					}
				}
				bool isAttachedToPayPlan=PayPlanLinks.GetForFKeyAndLinkType(_adjustment.AdjNum,PayPlanLinkType.Adjustment).Count>0;
				_listPaySplitsForAdjustment=PaySplits.GetForAdjustments(new List<long>() {_adjustment.AdjNum});
				if(_listPaySplitsForAdjustment.Count>0 || isAttachedToPayPlan) {
					butAttachProc.Enabled=false;
					butDetachProc.Enabled=false;
					labelProcDisabled.Visible=true;
				}
				//Do not let the user change the adjustment type if the current adjustment is a "discount plan" adjustment type.
				if(Defs.GetValue(DefCat.AdjTypes,_adjustment.AdjType)=="dp") {
					labelAdditions.Text=Lan.g(this,"Discount Plan")+": "+Defs.GetName(DefCat.AdjTypes,_adjustment.AdjType);
					labelSubtractions.Visible=false;
					listTypePos.Visible=false;
					listTypeNeg.Visible=false;
				}
			}
			textDateEntry.Text=_adjustment.DateEntry.ToShortDateString();
			textAdjDate.Text=_adjustment.AdjDate.ToShortDateString();
			if(_adjustment.ProcDate.Year > 1880) {
				textProcDate.Text=_adjustment.ProcDate.ToShortDateString();
			}
			if(_adjustment.ProcNum!=0) {
				textProcDate.ReadOnly=true;
				butDetachProc.Enabled=true;
			}
			if(Defs.GetValue(DefCat.AdjTypes,_adjustment.AdjType)=="+"){//pos
				textAmount.Text=_adjustment.AdjAmt.ToString("F");
			}
			else if(Defs.GetValue(DefCat.AdjTypes,_adjustment.AdjType)=="-"){//neg
				textAmount.Text=(-_adjustment.AdjAmt).ToString("F");//shows without the neg sign
			}
			else if(Defs.GetValue(DefCat.AdjTypes,_adjustment.AdjType)=="dp") {//Discount Plan (neg)
				textAmount.Text=(-_adjustment.AdjAmt).ToString("F");//shows without the neg sign
			}
			comboClinic.ClinicNumSelected=_adjustment.ClinicNum;
			comboProv.SetSelectedProvNum(_adjustment.ProvNum);
			FillComboProv();
			if(_adjustment.ProcNum!=0 && PrefC.GetInt(PrefName.RigorousAdjustments)==(int)RigorousAdjustments.EnforceFully) {
				comboProv.Enabled=false;
				butPickProv.Enabled=false;
				comboClinic.Enabled=false;
				if(Security.IsAuthorized(EnumPermType.Setup,true)) {
					labelEditAnyway.Visible=true;
					butEditAnyway.Visible=true;
				}
			}
			checkOnlyTsiExcludedAdjTypes.CheckedChanged-=checkOnlyTsiExcludedAdjTypes_Checked;
			List<ProgramProperty> listProgramPropertiesExcludedAdjTypes=ProgramProperties
				.GetWhere(x => x.ProgramNum==_program.ProgramNum
					&& _program.Enabled
					&& (x.PropertyDesc==ProgramProperties.PropertyDescs.TransWorld.SyncExcludePosAdjType
						|| x.PropertyDesc==ProgramProperties.PropertyDescs.TransWorld.SyncExcludeNegAdjType));
			//use guar's clinic if clinics are enabled and props for that clinic exist, otherwise use ClinicNum 0
			List<ProgramProperty> listProgramPropertiesForClinicExcludedAdjTypes=listProgramPropertiesExcludedAdjTypes.FindAll(x => x.ClinicNum==_patientGuar.ClinicNum);
			if(!PrefC.HasClinicsEnabled || listProgramPropertiesForClinicExcludedAdjTypes.Count==0) {
				listProgramPropertiesForClinicExcludedAdjTypes=listProgramPropertiesExcludedAdjTypes.FindAll(x => x.ClinicNum==0);
			}
			_listTsiExcludedAdjDefNums=listProgramPropertiesForClinicExcludedAdjTypes.Select(x=>PIn.Long(x.PropertyValue,false)).ToList();
			if(_program.Enabled && Patients.IsGuarCollections(_patientGuar.PatNum) && _listTsiExcludedAdjDefNums.Any(x => x>0)) { //Transworld program link is enabled and the patient is part of a family where the guarantor has been sent to TSI
				checkOnlyTsiExcludedAdjTypes.Checked=true;
			}
			else {
				checkOnlyTsiExcludedAdjTypes.Visible=false;
				checkOnlyTsiExcludedAdjTypes.Checked=false;
			}
			FillListBoxAdjTypes();
			checkOnlyTsiExcludedAdjTypes.CheckedChanged+=checkOnlyTsiExcludedAdjTypes_Checked;
			FillProcedure();
			textNote.Text=_adjustment.AdjNote;
		}

		private void listTypePos_SelectedIndexChanged(object sender,System.EventArgs e) {
			if(listTypePos.SelectedIndex>-1) {
				listTypeNeg.SelectedIndex=-1;
				FillProcedure();
			}
		}

		private void listTypeNeg_SelectedIndexChanged(object sender,System.EventArgs e) {
			if(listTypeNeg.SelectedIndex>-1) {
				listTypePos.SelectedIndex=-1;
				FillProcedure();
			}
		}

		private void textAmount_Validating(object sender,CancelEventArgs e) {
			FillProcedure();
		}

		private void butPickProv_Click(object sender,EventArgs e) {
			FrmProviderPick frmProviderPick = new FrmProviderPick(comboProv.Items.GetAll<Provider>());
			frmProviderPick.ProvNumSelected=comboProv.GetSelectedProvNum();
			frmProviderPick.ShowDialog();
			if(!frmProviderPick.IsDialogOK) {
				return;
			}
			comboProv.SetSelectedProvNum(frmProviderPick.ProvNumSelected);
		}

		private void comboClinic_SelectedIndexChanged(object sender,EventArgs e) {
			
		}

		private void ComboClinic_SelectionChangeCommitted(object sender, EventArgs e){
			FillComboProv();
		}

		///<summary>Fills combo provider based on which clinic is selected and attempts to preserve provider selection if any.</summary>
		private void FillComboProv() {
			long provNum=comboProv.GetSelectedProvNum();
			comboProv.Items.Clear();
			comboProv.Items.AddProvsAbbr(Providers.GetProvsForClinic(comboClinic.ClinicNumSelected));
			comboProv.SetSelectedProvNum(provNum);
		}

		private void FillProcedure(){
			if(_adjustment.ProcNum==0) {
				textProcDate2.Text="";
				textProcProv.Text="";
				textProcTooth.Text="";
				textProcDescription.Text="";
				textProcFee.Text="";
				textProcWriteoff.Text="";
				textProcInsPaid.Text="";
				textProcInsEst.Text="";
				textProcAdj.Text="";
				textProcPatPaid.Text="";
				textProcAdjCur.Text="";
				labelProcRemain.Text="";
				_adjRemAmt=0;
				return;
			}
			Procedure procedure=Procedures.GetOneProc(_adjustment.ProcNum,false);
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(procedure.PatNum);
			List<Adjustment> listAdjustments=Adjustments.Refresh(procedure.PatNum)
				.Where(x => x.ProcNum==procedure.ProcNum && x.AdjNum!=_adjustment.AdjNum).ToList();
			textProcDate.Text=procedure.ProcDate.ToShortDateString();	
			textProcDate2.Text=procedure.ProcDate.ToShortDateString();
			textProcProv.Text=Providers.GetAbbr(procedure.ProvNum);
			textProcTooth.Text=Tooth.Display(procedure.ToothNum);
			textProcDescription.Text=ProcedureCodes.GetProcCode(procedure.CodeNum).Descript;
			double procWO=-ClaimProcs.ProcWriteoff(listClaimProcs,procedure.ProcNum);
			double procInsPaid=-ClaimProcs.ProcInsPay(listClaimProcs,procedure.ProcNum);
			double procInsEst=-ClaimProcs.ProcEstNotReceived(listClaimProcs,procedure.ProcNum);
			double procAdj=listAdjustments.Sum(x => x.AdjAmt);
			double procPatPaid=-PaySplits.GetTotForProc(procedure);
			textProcFee.Text=procedure.ProcFeeTotal.ToString("F");
			textProcWriteoff.Text=procWO==0?"":procWO.ToString("F");
			textProcInsPaid.Text=procInsPaid==0?"":procInsPaid.ToString("F");
			textProcInsEst.Text=procInsEst==0?"":procInsEst.ToString("F");
			textProcAdj.Text=procAdj==0?"":procAdj.ToString("F");
			textProcPatPaid.Text=procPatPaid==0?"":procPatPaid.ToString("F");
			//Intelligently sum the values above based on statuses instead of blindly adding all of the values together.
			//The remaining amount is typically called the "patient portion" so utilze the centralized method that gets the patient portion.
			decimal patPort=ClaimProcs.GetPatPortion(procedure,listClaimProcs,listAdjustments);
			double procAdjCur=0;
			if(textAmount.IsValid()){
				if(listTypePos.SelectedIndex>-1){//pos
					procAdjCur=PIn.Double(textAmount.Text);
				}
				else if(listTypeNeg.SelectedIndex>-1 || Defs.GetValue(DefCat.AdjTypes,_adjustment.AdjType)=="dp"){//neg or discount plan
					procAdjCur=-PIn.Double(textAmount.Text);
				}
			}
			_isNegativeAdjustment=procAdjCur<0;
			textProcAdjCur.Text=procAdjCur==0?"":procAdjCur.ToString("F");
			//Add the current adjustment amount to the patient portion which will give the newly calculated remaining amount.
			_adjRemAmt=(decimal)procAdjCur+(decimal)procPatPaid+patPort;
			labelProcRemain.Text=_adjRemAmt.ToString("c");
		}

		private void butAttachProc_Click(object sender, System.EventArgs e) {
			using FormProcSelect formProcSelect=new FormProcSelect(_adjustment.PatNum,false,doShowTreatmentPlanProcs:false);
			formProcSelect.ShowDialog();
			if(formProcSelect.DialogResult!=DialogResult.OK){
				return;
			}
			if(OrthoProcLinks.IsProcLinked(formProcSelect.ListProceduresSelected[0].ProcNum)) {
				MsgBox.Show(this,"Adjustments cannot be attached to a procedure that is linked to an ortho case.");
				return;
			}
			//No need to check for completed status. Only allowed to select completed procedures.
			if(!Security.IsAuthorized(EnumPermType.ProcCompleteAddAdj,Procedures.GetDateForPermCheck(formProcSelect.ListProceduresSelected[0]))) {
				return;
			}
			if(PrefC.GetInt(PrefName.RigorousAdjustments)<2) {//Enforce Linking
				//_selectedProvNum=FormPS.ListSelectedProcs[0].ProvNum;
				comboClinic.ClinicNumSelected=formProcSelect.ListProceduresSelected[0].ClinicNum;
				comboProv.SetSelectedProvNum(formProcSelect.ListProceduresSelected[0].ProvNum);
				if(PrefC.GetInt(PrefName.RigorousAdjustments)==(int)RigorousAdjustments.EnforceFully && !_isEditAnyway) {
					if(Security.IsAuthorized(EnumPermType.Setup,true)) {
						labelEditAnyway.Visible=true;
						butEditAnyway.Visible=true;
					}
					comboProv.Enabled=false;//Don't allow changing if enforce fully
					butPickProv.Enabled=false;
					comboClinic.Enabled=false;//this is a separate issue from the internal edit blocking for comboClinic with no permission for a clinic
				}
			}
			_adjustment.ProcNum=formProcSelect.ListProceduresSelected[0].ProcNum;
			FillProcedure();
			textProcDate.Text=formProcSelect.ListProceduresSelected[0].ProcDate.ToShortDateString();
			textProcDate.ReadOnly=true;
		}

		private void butDetachProc_Click(object sender, System.EventArgs e) {
			textProcDate.ReadOnly=false;
			comboProv.Enabled=true;
			butPickProv.Enabled=true;
			comboClinic.Enabled=true;
			labelEditAnyway.Visible=false;
			butEditAnyway.Visible=false;
			_adjustment.ProcNum=0;
			FillProcedure();
		}

		private void butEditAnyway_Click(object sender,EventArgs e) {
			_isEditAnyway=true;
			comboClinic.Enabled=true;
			comboProv.Enabled=true;
			butPickProv.Enabled=true;
			labelEditAnyway.Visible=false;
			butEditAnyway.Visible=false;
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(!textAdjDate.IsValid() || !textProcDate.IsValid() || !textAmount.IsValid()) {
				MessageBox.Show(Lan.g(this,"Please fix data entry error first."));
				return;
			}
			if(Security.IsGlobalDateLock(EnumPermType.AdjustmentEdit,textAdjDate.Value)) {
				return;
			}
			if(textAmount.Value!=0 || !Security.IsAuthorized(EnumPermType.AdjustmentEditZero)) {//if it's a $0 adjustment and they have that permission we don't care about AdjustmentCreate
				if(IsNew && !Security.IsAuthorized(EnumPermType.AdjustmentCreate,textAdjDate.Value,false)) {
					return;
				}
			}
			bool isDiscountPlanAdj=(Defs.GetValue(DefCat.AdjTypes,_adjustment.AdjType)=="dp");
			if(PIn.Date(textAdjDate.Text).Date > DateTime.Today.Date && !PrefC.GetBool(PrefName.FutureTransDatesAllowed)) {
				MsgBox.Show(this,"Adjustment date can not be in the future.");
				return;
			}
			if(textAmount.Text==""){
				MessageBox.Show(Lan.g(this,"Please enter an amount."));	
				return;
			}
			if(!isDiscountPlanAdj && listTypeNeg.SelectedIndex==-1 && listTypePos.SelectedIndex==-1){
				MsgBox.Show(this,"Please select a type first.");
				return;
			}
			if(IsNew && AvaTax.IsEnabled() && listTypePos.SelectedIndex>-1 && 
				(_listDefsAdjPosCats[listTypePos.SelectedIndex].DefNum==AvaTax.GetSalesTaxAdjType() || _listDefsAdjPosCats[listTypePos.SelectedIndex].DefNum==AvaTax.GetSalesTaxReturnAdjType()) && 
				!Security.IsAuthorized(EnumPermType.SalesTaxAdjEdit))
			{
				return;
			}
			if(PrefC.GetInt(PrefName.RigorousAdjustments)==0 && _adjustment.ProcNum==0) {
				MsgBox.Show(this,"You must attach a procedure to the adjustment.");
				return;
			}
			PayPlanLink payPlanLink=PayPlanLinks.GetForFKeyAndLinkType(_adjustment.AdjNum,PayPlanLinkType.Adjustment).FirstOrDefault();
			bool isDPP=false;
			if(payPlanLink!=null) {
				PayPlan payPlan=PayPlans.GetOne(payPlanLink.PayPlanNum);
				isDPP=payPlan.IsDynamic;
			}
			if(isDPP) { //No longer allowing negative, unallocated adjustments on DPPs.
				double value=PIn.Double(textAmount.Text);
				//If a Subtraction adjustment type is selected, make value negative.
				if(listTypeNeg.SelectedIndex!=-1) {
					value*=-1;
				}
				if(value<0) {
					MsgBox.Show(this, "This adjustment is attached to a payment plan and it cannot be negative.");
					return;
				}
			}
			if(_adjRemAmt<0 && _isNegativeAdjustment) {
				EnumAdjustmentBlockOrWarn enumAdjustmentBlockOrWarn=PrefC.GetEnum<EnumAdjustmentBlockOrWarn>(PrefName.AdjustmentBlockNegativeExceedingPatPortion);
				if(enumAdjustmentBlockOrWarn==EnumAdjustmentBlockOrWarn.Warn) {
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Remaining amount is negative.  Continue?","Overpaid Procedure Warning")) {
						return;
					}
				}
				if(enumAdjustmentBlockOrWarn==EnumAdjustmentBlockOrWarn.Block) {
					MsgBox.Show(this,"Cannot create a negative adjustment exceeding the remaining amount on the procedure.","Overpaid Procedure Warning");
					return;
				}
			}
			bool changeAdjSplit=false;
			List<PaySplit> listPaySplitsForAdjust=new List<PaySplit>();
			if(IsNew){
				//prevents backdating of initial adjustment
				if(!Security.IsAuthorized(EnumPermType.AdjustmentCreate,PIn.Date(textAdjDate.Text),true)){//Give message later.
					if(!_checkZeroAmount) {//Let user create as long as Amount is zero and has edit zero permissions.  This was checked on load.
						MessageBox.Show(Lans.g("Security","Not authorized for")+"\r\n"+GroupPermissions.GetDesc(EnumPermType.AdjustmentCreate));
						return;
					}
				}
			}
			else{
				//Editing an old entry will already be blocked if the date was too old, and user will not be able to click OK button
				//This catches it if user changed the date to be older.
				if(!Security.IsAuthorized(EnumPermType.AdjustmentEdit,PIn.Date(textAdjDate.Text))){
					return;
				}
				if(_adjustment.ProvNum!=comboProv.GetSelectedProvNum()) {
					listPaySplitsForAdjust=PaySplits.GetForAdjustments(new List<long>() {_adjustment.AdjNum});
					for(int i=0;i<listPaySplitsForAdjust.Count;i++) {
						if(!Security.IsAuthorized(EnumPermType.PaymentEdit,Payments.GetPayment(listPaySplitsForAdjust[i].PayNum).PayDate)) {
							return;
						}
						if(comboProv.GetSelectedProvNum()!=listPaySplitsForAdjust[i].ProvNum && PrefC.GetInt(PrefName.RigorousAccounting)==(int)RigorousAdjustments.EnforceFully) {
							changeAdjSplit=true;
							break;
						}
					}
					if(changeAdjSplit
						&& !MsgBox.Show(this,MsgBoxButtons.OKCancel,"The provider for the associated payment splits will be changed to match the provider on the "
						+"adjustment.")) 
					{
						return;
					}
				}
			}
			//DateEntry not allowed to change
			DateTime datePreviousChange=_adjustment.SecDateTEdit;
			_adjustment.AdjDate=PIn.Date(textAdjDate.Text);
			_adjustment.ProcDate=PIn.Date(textProcDate.Text);
			_adjustment.ProvNum=comboProv.GetSelectedProvNum();
			_adjustment.ClinicNum=comboClinic.ClinicNumSelected;
			if(listTypePos.SelectedIndex!=-1) {
				_adjustment.AdjType=_listDefsAdjPosCats[listTypePos.SelectedIndex].DefNum;
				_adjustment.AdjAmt=PIn.Double(textAmount.Text);
			}
			if(listTypeNeg.SelectedIndex!=-1) {
				_adjustment.AdjType=_listDefsAdjNegCats[listTypeNeg.SelectedIndex].DefNum;
				_adjustment.AdjAmt=-PIn.Double(textAmount.Text);
			}
			if(isDiscountPlanAdj) {
				//AdjustmentCur.AdjType is already set to a "discount plan" adj type.
				_adjustment.AdjAmt=-PIn.Double(textAmount.Text);
			}
			if(_checkZeroAmount && _adjustment.AdjAmt!=0) {
				MsgBox.Show(this,"Amount has to be 0.00 due to security permission.");
				return;
			}
			if(_program.Enabled && Patients.IsGuarCollections(_patientGuar.PatNum) && _listTsiExcludedAdjDefNums.Any(x => x>0)) {
				if(checkOnlyTsiExcludedAdjTypes.Checked && !MsgBox.Show(this,MsgBoxButtons.OKCancel,"The guarantor of this family has been sent to TSI for a past due balance and you have selected an adjustment type that is excluded from being synched with TSI. This will not reduce the balance sent for collection by TSI. Continue?")){
					return;
				}
				if(!checkOnlyTsiExcludedAdjTypes.Checked && !MsgBox.Show(this,MsgBoxButtons.OKCancel,"The guarantor of this family has been sent to TSI for a past due balance and you have selected an adjustment type that will be synched with TSI. This balance adjustment could result in a TSI charge for collection. Continue?")){
					return;
				}
			}
			_adjustment.AdjNote=textNote.Text;
			if(IsNew) {
				try{
					Adjustments.Insert(_adjustment);
				}
				catch(Exception ex){//even though it doesn't currently throw any exceptions
					MessageBox.Show(ex.Message);
					return;
				}
				SecurityLogs.MakeLogEntry(EnumPermType.AdjustmentCreate,_adjustment.PatNum,
					_patient.GetNameLF()+", "
					+_adjustment.AdjAmt.ToString("c"));
				TsiTransLogs.CheckAndInsertLogsIfAdjTypeExcluded(_adjustment,_isTsiAdj);
			}
			else {
				try{ 
					Adjustments.Update(_adjustment);
				}
				catch(Exception ex){//even though it doesn't currently throw any exceptions
					MessageBox.Show(ex.Message);
					return;
				}
				SecurityLogs.MakeLogEntry(EnumPermType.AdjustmentEdit,_adjustment.PatNum,_patient.GetNameLF()+", "+_adjustment.AdjAmt.ToString("c"),0,datePreviousChange);
			}
			if(changeAdjSplit) {
				PaySplits.UpdateProvForAdjust(_adjustment,listPaySplitsForAdjust);
			}
			Signalods.SetInvalid(InvalidType.BillingList);
			DialogResult=DialogResult.OK;
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(_listPaySplitsForAdjustment.Count>0) {
				MsgBox.Show(this,"Cannot delete adjustment while a payment split is attached.");
				return;
			}
			bool isAttachedToPayPlan=PayPlanLinks.GetForFKeyAndLinkType(_adjustment.AdjNum,PayPlanLinkType.Adjustment).Count>0;
			if(isAttachedToPayPlan) {
				MsgBox.Show(this,"Cannot delete adjustment that is attached to a payment plan.");
				return;
			}
			if(_listPaySplitsForAdjustment.Count>0 
				&& !MsgBox.Show(this,MsgBoxButtons.YesNo,"There are payment splits associated to this adjustment.  Do you want to continue deleting?")) 
			{//There are splits for this adjustment
				return;
			}
			SecurityLogs.MakeLogEntry(EnumPermType.AdjustmentEdit,_adjustment.PatNum
				,"Delete for patient: "+_patient.GetNameLF()+", "+_adjustment.AdjAmt.ToString("c"),0,_adjustment.SecDateTEdit);
			Adjustments.Delete(_adjustment);
			Signalods.SetInvalid(InvalidType.BillingList);
			DialogResult=DialogResult.OK;
		}

		private void FillListBoxAdjTypes() {
			listTypePos.Items.Clear();
			listTypeNeg.Items.Clear();
			//prevents FillProcedure from being called too many times.  Event handlers hooked back up after the lists are filled.
			listTypeNeg.SelectedIndexChanged-=listTypeNeg_SelectedIndexChanged;
			listTypePos.SelectedIndexChanged-=listTypePos_SelectedIndexChanged;
			//Positive adjustment types
			_listDefsAdjPosCats=Defs.GetPositiveAdjTypes(considerPermission:true);
			if(checkOnlyTsiExcludedAdjTypes.Checked) {
				_listDefsAdjPosCats=_listDefsAdjPosCats.FindAll(x=>_listTsiExcludedAdjDefNums.Contains(x.DefNum));
			}
			else {
				_listDefsAdjPosCats=_listDefsAdjPosCats.FindAll(x=> !_listTsiExcludedAdjDefNums.Contains(x.DefNum));
			}
			_listDefsAdjPosCats.ForEach(x => listTypePos.Items.Add(x.ItemName));
			listTypePos.SelectedIndex=_listDefsAdjPosCats.FindIndex(x => x.DefNum==_adjustment.AdjType);//can be -1
			//Negative adjustment types
			_listDefsAdjNegCats=Defs.GetNegativeAdjTypes(considerPermission:true);
			if(checkOnlyTsiExcludedAdjTypes.Checked) {
				_listDefsAdjNegCats=_listDefsAdjNegCats.FindAll(x => _listTsiExcludedAdjDefNums.Contains(x.DefNum));
			}
			else {
				_listDefsAdjNegCats=_listDefsAdjNegCats.FindAll(x=> !_listTsiExcludedAdjDefNums.Contains(x.DefNum));
			}
			_listDefsAdjNegCats.ForEach(x => listTypeNeg.Items.Add(x.ItemName));
			listTypeNeg.SelectedIndex=_listDefsAdjNegCats.FindIndex(x => x.DefNum==_adjustment.AdjType);//can be -1
			listTypeNeg.SelectedIndexChanged+=listTypeNeg_SelectedIndexChanged;
			listTypePos.SelectedIndexChanged+=listTypePos_SelectedIndexChanged;
		}

		private void checkOnlyTsiExcludedAdjTypes_Checked(object sender,EventArgs e) {
			FillListBoxAdjTypes();
		}
	}

	///<summary></summary>
	public struct AdjustmentItem{
		///<summary></summary>
		public string ItemText;
		///<summary></summary>
		public int ItemIndex;
	}

}
