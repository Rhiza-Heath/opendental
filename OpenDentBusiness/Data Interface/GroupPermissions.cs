using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Windows.Forms;
using System.Linq;
using CodeBase;
namespace OpenDentBusiness{
	///<summary></summary>
	public class GroupPermissions {
		///<summary>The maximum number of days allowed for the NewerDays column.
		///Setting a NewerDays to a value higher than this will cause an exception to be thrown in the program.
		///There is a DBM that will correct invalid NewerDays in the database.</summary>
		public const double NewerDaysMax=3000;

		#region Misc Methods
		///<summary>Returns the Date that the user is restricted to for the passed-in permission. 
		///Returns MinVal if the user is not restricted or does not have the permission.</summary>
		public static DateTime GetDateRestrictedForPermission(EnumPermType permission,List<long> listUserGroupNums) {
			//No need to check MiddleTierRole; no call to db.
			DateTime nowDate=DateTime.MinValue;
			Func<DateTime> getNowDate=new Func<DateTime>(() => {
				if(nowDate.Year < 1880) {
					nowDate=MiscData.GetNowDateTime().Date;
				}
				return nowDate;
			});
			DateTime retVal=DateTime.MinValue;
			List<GroupPermission> listGroupPerms=GetForUserGroups(listUserGroupNums,permission);
			//get the permission that applies
			GroupPermission perm=listGroupPerms.OrderBy((GroupPermission y) => {
				if(y.NewerDays==0 && y.NewerDate==DateTime.MinValue) {
					return DateTime.MinValue;
				}
				if(y.NewerDays==0) {
					return y.NewerDate;
				}
				return getNowDate().AddDays(-y.NewerDays);
			}).FirstOrDefault();
			if(perm==null) {
				//do not change retVal. The user does not have the permission.
			}
			else if(perm.NewerDate.Year < 1880 && perm.NewerDays == 0) {
				//do not change retVal. The user is not restricted by date.
			}
			else if(perm.NewerDate.Year > 1880) {
				retVal=perm.NewerDate;
			}
			else if(getNowDate().AddDays(-perm.NewerDays)>retVal) {
				retVal=getNowDate().AddDays(-perm.NewerDays);
			}
			return retVal;
		}

		///<summary>Used for procedures with status EO, EC, or C. Returns Permissions.ProcExistingEdit for EO/EC</summary>
		public static EnumPermType SwitchExistingPermissionIfNeeded(EnumPermType perm,Procedure proc) {
			if(proc.ProcStatus.In(ProcStat.EO,ProcStat.EC)) {
				return EnumPermType.ProcExistingEdit;
			}
			return perm;
		}
		#endregion

		#region CachePattern

		private class GroupPermissionCache : CacheListAbs<GroupPermission> {
			protected override List<GroupPermission> GetCacheFromDb() {
				string command="SELECT * FROM grouppermission";
				return Crud.GroupPermissionCrud.SelectMany(command);
			}
			protected override List<GroupPermission> TableToList(DataTable table) {
				return Crud.GroupPermissionCrud.TableToList(table);
			}
			protected override GroupPermission Copy(GroupPermission GroupPermission) {
				return GroupPermission.Copy();
			}
			protected override DataTable ListToTable(List<GroupPermission> listGroupPermissions) {
				return Crud.GroupPermissionCrud.ListToTable(listGroupPermissions,"GroupPermission");
			}
			protected override void FillCacheIfNeeded() {
				GroupPermissions.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static GroupPermissionCache _GroupPermissionCache=new GroupPermissionCache();

		public static GroupPermission GetFirstOrDefault(Func<GroupPermission,bool> match,bool isShort=false) {
			return _GroupPermissionCache.GetFirstOrDefault(match,isShort);
		}

		public static List<GroupPermission> GetWhere(Predicate<GroupPermission> match,bool isShort=false) {
			return _GroupPermissionCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_GroupPermissionCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_GroupPermissionCache.FillCacheFromTable(table);
				return table;
			}
			return _GroupPermissionCache.GetTableFromCache(doRefreshCache);
		}

		///<summary>Clears the cache.</summary>
		public static void ClearCache() {
			_GroupPermissionCache.ClearCache();
		}
		#endregion

		///<summary></summary>
		public static void Update(GroupPermission gp){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),gp);
				return;
			}
			if(gp.NewerDate.Year>1880 && gp.NewerDays>0) {
				throw new Exception(Lans.g("GroupPermissions","Date or days can be set, but not both."));
			}
			if(!GroupPermissions.PermTakesDates(gp.PermType)) {
				if(gp.NewerDate.Year>1880 || gp.NewerDays>0) {
					throw new Exception(Lans.g("GroupPermissions","This type of permission may not have a date or days set."));
				}
			}
			Crud.GroupPermissionCrud.Update(gp);
		}

		///<summary>Update that doesn't use the local cache or validation. Useful for multiple database connections.</summary>
		public static void UpdateNoCache(GroupPermission gp) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),gp);
				return;
			}
			Crud.GroupPermissionCrud.Update(gp);
		}

		///<summary>Deletes GroupPermissions based on primary key.  Do not call this method unless you have checked specific dependencies first.  E.g. after deleting this permission, there will still be a security admin user.  This method is only called from the CEMT sync.  RemovePermission should probably be used instead.</summary>
		public static void Delete(GroupPermission gp) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),gp);
				return;
			}
			string command="DELETE FROM grouppermission WHERE GroupPermNum = "+POut.Long(gp.GroupPermNum);
			Db.NonQ(command);
		}

		///<summary>Deletes without using the cache. Cannot trust GroupPermNum when dealing with remote DB so we rely on every other field to check.</summary>
		public static void DeleteNoCache(GroupPermission gp) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),gp);
				return;
			}
			string command=$@"DELETE FROM grouppermission 
				WHERE NewerDate={POut.Date(gp.NewerDate)} 
				AND NewerDays={POut.Int(gp.NewerDays)} 
				AND UserGroupNum={POut.Long(gp.UserGroupNum)} 
				AND PermType={POut.Int((int)gp.PermType)} 
				AND FKey={POut.Long(gp.FKey)}";
			Db.NonQ(command);
		}

		///<summary>Delete all GroupPermissions for the specified PermType and UserGroupNum.</summary>
		public static void DeleteForPermTypeAndUserGroup(EnumPermType permType,long userGroupNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),permType,userGroupNum);
				return;
			}
			string command="DELETE FROM grouppermission WHERE PermType="+POut.Enum<EnumPermType>(permType)+" AND UserGroupNum="+POut.Long(userGroupNum);
			Db.NonQ(command);
		}

		///<summary></summary>
		public static long Insert(GroupPermission gp){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				gp.GroupPermNum=Meth.GetLong(MethodBase.GetCurrentMethod(),gp);
				return gp.GroupPermNum;
			}
			if(gp.NewerDate.Year>1880 && gp.NewerDays>0) {
				throw new Exception(Lans.g("GroupPermissions","Date or days can be set, but not both."));
			}
			if(!GroupPermissions.PermTakesDates(gp.PermType)) {
				if(gp.NewerDate.Year>1880 || gp.NewerDays>0) {
					throw new Exception(Lans.g("GroupPermissions","This type of permission may not have a date or days set."));
				}
			}
			if(gp.PermType==EnumPermType.SecurityAdmin) {
				//Make sure there are no hidden users in the group that is about to get the Security Admin permission.
				string command="SELECT COUNT(*) FROM userod "
					+"INNER JOIN usergroupattach ON usergroupattach.UserNum=userod.UserNum "
					+"WHERE userod.IsHidden=1 "
					+"AND usergroupattach.UserGroupNum="+gp.UserGroupNum;
				int count=PIn.Int(Db.GetCount(command));
				if(count!=0) {//there are hidden users in this group
					throw new Exception(Lans.g("FormSecurity","The Security Admin permission cannot be given to a user group with hidden users."));
				}
			}
			return Crud.GroupPermissionCrud.Insert(gp);
		}

		///<summary>Insertion logic that doesn't use the cache. Always ignores the PK and relies on auto-increment.</summary>
		public static long InsertNoCache(GroupPermission gp) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),gp);
			}
			string command="INSERT INTO grouppermission (NewerDate,NewerDays,UserGroupNum,PermType,FKey) VALUES (" 
				+    POut.Date  (gp.NewerDate)+","
				+    POut.Int   (gp.NewerDays)+","
				+    POut.Long  (gp.UserGroupNum)+","
				+    POut.Int   ((int)gp.PermType)+","
				+    POut.Long  (gp.FKey)+")";
			return Db.GetLong(command);
		}

		///<summary></summary>
		public static void RemovePermission(long groupNum,EnumPermType permType) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),groupNum,permType);
				return;
			}
			string command;
			if(permType==EnumPermType.SecurityAdmin){
				//need to make sure that at least one other user has this permission
				command="SELECT COUNT(*) FROM (SELECT DISTINCT grouppermission.UserGroupNum "
					+"FROM grouppermission "
					+"INNER JOIN usergroupattach ON usergroupattach.UserGroupNum=grouppermission.UserGroupNum "
					+"INNER JOIN userod ON userod.UserNum=usergroupattach.UserNum AND userod.IsHidden=0 "
					+"WHERE grouppermission.PermType='"+POut.Long((int)permType)+"' "
					+"AND grouppermission.UserGroupNum!="+POut.Long(groupNum)+") t";//This query is Oracle compatable
				if(Db.GetScalar(command)=="0") {//no other users outside of this group have SecurityAdmin
					throw new Exception(Lans.g("FormSecurity","There must always be at least one user in a user group that has the Security Admin permission."));
				}
			}
			command="DELETE FROM grouppermission WHERE UserGroupNum="+POut.Long(groupNum)+" "
				+"AND PermType="+POut.Long((int)permType);
 			Db.NonQ(command);
		}

		public static bool Sync(List<GroupPermission> listNew,List<GroupPermission> listOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listNew,listOld);
			}
			return Crud.GroupPermissionCrud.Sync(listNew,listOld);
		}

		///<summary>Gets a GroupPermission based on the supplied userGroupNum and permType.  If not found, then it returns null.  Used in FormSecurity when double clicking on a dated permission or when clicking the all button.</summary>
		public static GroupPermission GetPerm(long userGroupNum,EnumPermType permType) {
			//No need to check MiddleTierRole; no call to db.
			return GetFirstOrDefault(x => x.UserGroupNum==userGroupNum && x.PermType==permType);
		}

		///<summary>Gets a list of GroupPermissions for the supplied UserGroupNum.</summary>
		public static List<GroupPermission> GetPerms(long userGroupNum) {
			//No need to check MiddleTierRole; no call to db.
			return GetWhere(x => x.UserGroupNum==userGroupNum);
		}

		///<summary>Gets a list of GroupPermissions for the supplied UserGroupNum without using the local cache.  Useful for multithreaded connections.</summary>
		public static List<GroupPermission> GetPermsNoCache(long userGroupNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<GroupPermission>>(MethodBase.GetCurrentMethod(),userGroupNum);
			}
			List<GroupPermission> retVal=new List<GroupPermission>();
			string command="SELECT * FROM grouppermission WHERE UserGroupNum="+POut.Long(userGroupNum);
			DataTable tableGroupPerms=Db.GetTable(command);
			retVal=Crud.GroupPermissionCrud.TableToList(tableGroupPerms);
			return retVal;
		}

		///<summary>Gets a list of GroupPermissions that are associated with reports. Uses Reports (22) permission.</summary>
		public static List<GroupPermission> GetPermsForReports(long userGroupNum=0) {
			//No need to check MiddleTierRole; no call to db.
			List<GroupPermission> listGroupPermissions=GetWhere(x => x.PermType==EnumPermType.Reports);
			if(userGroupNum > 0) {
				listGroupPermissions.RemoveAll(x => x.UserGroupNum!=userGroupNum);
			}
			return listGroupPermissions;
		}

		///<summary>Gets a list of AdjustmentTypeDeny perms for a user group. Having an AdjustmentTypeDeny perm indicates the user group does not have 
		///permission to access (create,edit,edit zero) the adjustmenttype that has a defnum==fkey. Pattern approved by Jordan.</summary>
		public static List<GroupPermission> GetAdjustmentTypeDenyPermsForUserGroup(long userGroupNum) {
			//No need to check MiddleTierRole; no call to db.
			return GetWhere(x => x.PermType==EnumPermType.AdjustmentTypeDeny && x.UserGroupNum==userGroupNum);
		}

		///<summary>Gets a list of GroupPermissions that are associated with reports and the user groups that the passed in user. Uses Reports (22) permission.</summary>
		public static List<GroupPermission> GetPermsForReports(Userod user) {
			//No need to check MiddleTierRole; no call to db.
			return GetWhere(x => x.PermType==EnumPermType.Reports && user.IsInUserGroup(x.UserGroupNum));
		}

		///<summary>Used to check if user has permission to access the report. Pass in a list of DisplayReports to avoid a call to the db.</summary>
		public static bool HasReportPermission(string reportName,Userod user,List<DisplayReport> listReports=null) {
			//No need to check MiddleTierRole; no call to db.
			DisplayReport report=(listReports??DisplayReports.GetAll(false)).FirstOrDefault(x=>x.InternalName==reportName);
			if(report==null) {//Report is probably hidden.
				return false;
			}
			List<GroupPermission> listReportPermissions=GroupPermissions.GetPermsForReports(user);
			return listReportPermissions.Any(x => x.FKey.In(0,report.DisplayReportNum));//Zero FKey means access to every report.
		}

		///<summary>Determines whether a single userGroup contains a specific permission.</summary>
		public static bool HasPermission(long userGroupNum,EnumPermType permType,long fKey,List<GroupPermission> listGroupPermissions=null) {
			//No need to check MiddleTierRole; no call to db.
			List<GroupPermission> listGroupPermissionsCopy;
			if(listGroupPermissions==null) {
				listGroupPermissionsCopy=GetWhere(x => x.UserGroupNum==userGroupNum && x.PermType==permType);
			}
			else {
				listGroupPermissionsCopy=new List<GroupPermission>(listGroupPermissions);
				listGroupPermissionsCopy.RemoveAll(x => x.UserGroupNum!=userGroupNum || x.PermType!=permType);
			}
			if(DoesPermissionTreatZeroFKeyAsAll(permType) && listGroupPermissionsCopy.Any(x => x.FKey==0)) {//Access to everything.
				return true;
			}
			return listGroupPermissionsCopy.Any(x => x.FKey==fKey);
		}

		///<summary>Determines whether an individual user has a specific permission.</summary>
		public static bool HasPermission(Userod user,EnumPermType permType,long fKey,List<GroupPermission> listGroupPermissions=null) {
			//No need to check MiddleTierRole; no call to db.
			if(listGroupPermissions==null) {
				listGroupPermissions=GetWhere(x => x.PermType==permType && user.IsInUserGroup(x.UserGroupNum));
			}
			else {
				listGroupPermissions.RemoveAll(x => x.PermType!=permType && !user.IsInUserGroup(x.UserGroupNum));
			}
			if(DoesPermissionTreatZeroFKeyAsAll(permType) && listGroupPermissions.Any(x => x.FKey==0)) {//Access to everything.
				return true;
			}
			return listGroupPermissions.Any(x => x.FKey==fKey);
		}

		///<summary>Checks if user has permission to access the passed-in adjustment type. 
		///Unlike other permissions, if this permission node isn't checked then a user is not barred from creating this specific adjustment type</summary>
		public static bool HasPermissionForAdjType(Def adjTypeDef,bool suppressMessage = true) {
			List<UserGroup> listUserGroupsAdjTypeDeny=UserGroups.GetForPermission(EnumPermType.AdjustmentTypeDeny);
			List<UserGroup> listUserGroupsForUser=UserGroups.GetForUser(Security.CurUser.UserNum, (Security.CurUser.UserNumCEMT!=0));
			List<UserGroup> listUserGroupsForUserWithAdjTypeDeny=listUserGroupsForUser.FindAll(x => listUserGroupsAdjTypeDeny.Any(y => y.UserGroupNum==x.UserGroupNum));
			List<long> listUserGroupNums=listUserGroupsForUserWithAdjTypeDeny.Select(x => x.UserGroupNum).ToList();
			List<GroupPermission> listGroupPermissions=GetForUserGroups(listUserGroupNums, EnumPermType.AdjustmentTypeDeny)
				.FindAll(x => x.FKey==adjTypeDef.DefNum || x.FKey==0);// Fkey of 0 means all adjTypeDefs were selected
			//Return true when not all the user's groups with AdjustmentTypeDeny have the adjTypeDef.DefNum checked or have the Fkey value of 0 so the adjustment is not blocked.
			if(listGroupPermissions.IsNullOrEmpty() || listGroupPermissions.Count!=listUserGroupsForUser.Count ) {
        return true;
			}
			if(suppressMessage) {
				return false;
			}
			string unauthorizedMessage=Lans.g("Security","Not authorized.")+"\r\n"
				+Lans.g("Security","A user with the SecurityAdmin permission must grant you access for adjustment type")+":\r\n"+adjTypeDef.ItemName;
      MessageBox.Show(unauthorizedMessage);
			return false;
		}

		///<summary>Checks if user has permission to access the passed-in adjustment type then checks if the user has the passed-in permission as well.</summary>
		public static bool HasPermissionForAdjType(EnumPermType permType,Def adjTypeDef,bool supressMessage=true) {
			return HasPermissionForAdjType(permType,adjTypeDef,DateTime.MinValue,supressMessage);
		}

		///<summary>Checks if user has permission to access the passed-in adjustment type then checks if the user has the passed-in permission as well. Use this method if the permission
		///also takes in a date.</summary>
		public static bool HasPermissionForAdjType(EnumPermType permType,Def adjTypeDef,DateTime dateTime,bool suppressMessage=true) {
			bool canEdit=HasPermissionForAdjType(adjTypeDef,suppressMessage);
			if(!canEdit) {
				return false;
			}
			return Security.IsAuthorized(permType,dateTime,suppressMessage);
		}

		public static bool DoesPermissionTreatZeroFKeyAsAll(EnumPermType permType) {
			return permType.In(EnumPermType.AdjustmentTypeDeny,EnumPermType.DashboardWidget,EnumPermType.Reports);
		}

		///<summary>Returns permissions associated to the passed-in usergroups. 
		///Pass in a specific permType to only return GroupPermissions of that type.
		///Otherwise, will return all GroupPermissions for the UserGroups.</summary>
		public static List<GroupPermission> GetForUserGroups(List<long> listUserGroupNums, EnumPermType permType=EnumPermType.None) {
			//No need to check MiddleTierRole; no call to db.
			if(permType==EnumPermType.None) {
				return GetWhere(x => listUserGroupNums.Contains(x.UserGroupNum));
			}
			return GetWhere(x => x.PermType == permType && listUserGroupNums.Contains(x.UserGroupNum));
		}

		///<summary>Gets permissions that actually generate audit trail entries.</summary>
		public static bool HasAuditTrail(EnumPermType permType) {
			//No need to check MiddleTierRole; no call to db.
			switch(permType) {//If commented, has an audit trail. In the order they appear in Permissions enumeration
				//Normal pattern is to comment out the FALSE cases. 
				//This is the opposite so that the default behavior for new security permissions to be to show in the audit trail. In case it wasn't added to this function.
				case EnumPermType.None:
				case EnumPermType.AppointmentsModule:
				//case Permissions.FamilyModule:
				//case Permissions.AccountModule:
				//case Permissions.TPModule:
				//case Permissions.ChartModule:
				//case Permissions.ImagesModule:
				case EnumPermType.ManageModule:
				//case Permissions.Setup:
				//case Permissions.RxCreate:
				//case Permissions.ChooseDatabase:
				//case Permissions.Schedules:
				//case Permissions.Blockouts:
				//case Permissions.ClaimSentEdit:
				//case Permissions.PaymentCreate:
				//case Permissions.PaymentEdit:
				//case Permissions.AdjustmentCreate:
				//case Permissions.AdjustmentEdit:
				//case Permissions.UserQuery:
				case EnumPermType.StartupSingleUserOld:
				case EnumPermType.StartupMultiUserOld:
				//case Permissions.Reports:
				//case Permissions.ProcComplCreate:
				//case Permissions.SecurityAdmin:
				//case Permissions.AppointmentCreate:
				//case Permissions.AppointmentMove:
				//case Permissions.AppointmentEdit:
				//case Permissions.AppointmentCompleteEdit:
				//case Permissions.Backup:
				case EnumPermType.TimecardsEditAll:
				//case Permissions.DepositSlips:
				//case Permissions.AccountingEdit:
				//case Permissions.AccountingCreate:
				//case Permissions.Accounting:
				case EnumPermType.AnesthesiaIntakeMeds:
				case EnumPermType.AnesthesiaControlMeds:
				//case Permissions.InsPayCreate:
				//case Permissions.InsPayEdit:
				//case Permissions.TreatPlanEdit:
				//case Permissions.ReportProdInc:
				//case Permissions.TimecardDeleteEntry:
				case EnumPermType.EquipmentDelete:
				//case Permissions.SheetEdit:
				//case Permissions.CommlogEdit:
				//case Permissions.ImageDelete:
				//case Permissions.PerioEdit:
				case EnumPermType.ProcEditShowFee:
				case EnumPermType.AdjustmentEditZero:
				case EnumPermType.EhrEmergencyAccess:
				//case Permissions.ProcDelete:
				case EnumPermType.EhrKeyAdd:
				//case Permissions.ProviderEdit:
				case EnumPermType.EcwAppointmentRevise:
				case EnumPermType.ProcedureNoteFull:
				case EnumPermType.ProcedureNoteUser:
				//case Permissions.ReferralAdd:
				//case Permissions.InsPlanChangeSubsc:
				//case Permissions.RefAttachAdd:
				//case Permissions.RefAttachDelete:
				//case Permissions.CarrierCreate:
				//case Permissions.CarrierEdit:
				case EnumPermType.GraphicalReports:
				//case Permissions.AutoNoteQuickNoteEdit:
				case EnumPermType.EquipmentSetup:
				//case Permissions.Billing:
				//case Permissions.ProblemDefEdit:
				//case Permissions.ProcFeeEdit:
				//case Permissions.InsPlanChangeCarrierName:
				//case Permissions.TaskNoteEdit:
				case EnumPermType.WikiListSetup:
				case EnumPermType.Copy:
				//case Permissions.Printing:
				//case Permissions.MedicalInfoViewed:
				//case Permissions.PatProblemListEdit:
				//case Permissions.PatMedicationListEdit:
				//case Permissions.PatAllergyListEdit:
				case EnumPermType.PatFamilyHealthEdit:
				case EnumPermType.PatientPortal:
				//case Permissions.RxEdit:
				case EnumPermType.AdminDentalStudents:
				case EnumPermType.AdminDentalInstructors:
				//case Permissions.OrthoChartEditFull:
				case EnumPermType.OrthoChartEditUser://We only ever use OrthoChartEditFull when audit trailing.
				//case Permissions.PatientFieldEdit:
				case EnumPermType.AdminDentalEvaluations:
				//case Permissions.TreatPlanDiscountEdit:
				//case Permissions.UserLogOnOff:
				//case Permissions.TaskEdit:
				//case Permissions.EmailSend:
				//case Permissions.WebmailSend:
				case EnumPermType.UserQueryAdmin:
				//case Permissions.InsPlanChangeAssign:
				//case Permissions.ImageEdit:
				//case Permissions.EhrMeasureEventEdit:
				//case Permissions.EServicesSetup:
				//case Permissions.FeeSchedEdit:
				//case Permissions.PatientBillingEdit:
				case EnumPermType.ProviderFeeEdit:
				case EnumPermType.ClaimHistoryEdit:
				//case Permissions.FeatureRequestEdit:
				//case Permissions.QueryRequestEdit:
				//case Permissions.JobApproval:
				//case Permissions.JobDocumentation:
				//case Permissions.JobEdit:
				//case Permissions.JobManager:
				//case Permissions.JobReview:
				//case Permissions.WebmailDelete:
				//case Permissions.MissingRequiredField:
				//case Permissions.ReferralMerge:
				//case Permissions.ProcEdit:
				//case Permissions.ProviderMerge:
				//case Permissions.MedicationMerge:
				//case Permissions.AccountQuickCharge:
				//case Permissions.ClaimSend:
				//case Permissions.TaskListCreate:
				//case Permissions.PatientCreate:
				//case Permissions.GraphicalReportSetup:
				case EnumPermType.PreAuthSentEdit:
				//case Permissions.PatientEdit:
				//case Permissions.InsPlanCreate:
				//case Permissions.InsPlanEdit:
				//case Permissions.InsPlanCreateSub:
				//case Permissions.InsPlanEditSub:
				//case Permissions.InsPlanAddPat:
				//case Permissions.InsPlanDropPat:
				case EnumPermType.InsPlanVerifyList:
				//case Permissions.SheetEdit:
				//case Permissions.SplitCreatePastLockDate:
				//case Permissions.ClaimDelete:
				//case Permissions.InsWriteOffEdit:
				case EnumPermType.ProviderAlphabetize:
				//case Permissions.ApptConfirmStatusEdit:
				//case Permissions.GraphicsRemoteEdit:
				//case Permissions.AuditTrail:
				//case Permissions.TreatPlanPresenterEdit:
				case EnumPermType.ClaimProcReceivedEdit:
				//case Permissions.MobileWeb:
				//case Permissions.StatementPatNumMismatch:
				//case Permissions.PatPriProvEdit:
				//case Permissions.ReferralEdit:
				//case Permissions.ReplicationSetup:
				case EnumPermType.ReportProdIncAllProviders:
				//case Permissions.ReportDaily:
				case EnumPermType.ReportDailyAllProviders:
				case EnumPermType.SheetDelete:
				case EnumPermType.UpdateCustomTracking:
				//case Permissions.GraphicsEdit:
				case EnumPermType.InsPlanOrthoEdit:
				//case Permissions.ClaimProcClaimAttachedProvEdit:
				//case Permissions.InsPlanMerge:
				//case Permissions.InsuranceCarrierCombine:
				case EnumPermType.PopupEdit://Popups are archived, so they don't need to show in the audit trail.
				case EnumPermType.InsPlanPickListExisting:
				case EnumPermType.GroupNoteEditSigned:
				case EnumPermType.WikiAdmin:
				//case Permissions.PayPlanEdit:
				//case Permissions.ClaimEdit:
				//case Permissions.LogFeeEdit:
				//case Permissions.LogSubscriberEdit:
				//case Permissions.RecallEdit:
				//case Permissions.ProcCodeEdit:
				//case Permissions.AddNewUser:
				case EnumPermType.ClaimView:
				//case Permissions.RepeatChargeTool:
				//case Permissions.DiscountPlanAddDrop:
				case EnumPermType.TreatPlanSign:
				case EnumPermType.UnrestrictedSearch:
				case EnumPermType.ArchivedPatientEdit:
				case EnumPermType.CommlogPersistent:
				//case Permissions.VerifyPhoneOwnership
				//case Permissions.SalesTaxAdjEdit://All other adjustment operations are already audited.
				//case Permissions.AgingRan:
				case EnumPermType.InsuranceVerification:
				//case Permissions.CreditCardMove:
				//case Permissions.HeadmasterSetup
				case EnumPermType.NewClaimsProcNotBilled:
				//case Permissions.PatientPortalLogin:
				//case Permissions.FAQEdit:
				case EnumPermType.FeatureRequestEdit:
				//case Permissions.SupplementalBackup:
				//case Permissions.WebSchedRecallManualSend:
				//case Permissions.PatientSSNView:
				//case Permissions.PatientDOBView:
				//case Permissions.FamAgingTruncate:
				//case Permissions.DiscountPlanMerge:
				//case Permissions.ProcCompleteEditMisc:
				//case Permissions.ProcCompleteAddAdj:
				//case Permissions.ProcCompletetStatusEdit:
				//case Permissions.ProcCompleteNote:
				//case Permissions.ProcCompleteEdit:
				//case Permissions.ProtectedLeaveAdjustmentEdit:
				//case Permissions.TimeAdjustEdit:
				//case Permissions.QueryMonitor:
				//case Permissions.CommlogCreate:
				//case Permissions.RepeatChargeCreate:
				//case Permissions.RepeatChargeUpdate:
				//case Permissions.RepeatChargeDelete:
				case EnumPermType.WebFormAccess:
				//case Permissions.CloseOtherSessions:
				case EnumPermType.Zoom:
				//case Permissions.ImageExport:
				//case Permissions.ImageCreate:
				case EnumPermType.CertificationEmployee:
				case EnumPermType.CertificationSetup:
				//case Permissions.AllowLoginFromAnywhere:
				//case Permissions.PayPlanChargeDateEdit;
				//case Permissions.DiscountPlanAdd:
				//case Permissions.DiscountPlanEdit:
				//case Permissions.AllowFeeEditWhileReceivingClaim:
				//case Permissions.ManageHighSecurityProgProperties:
				//case Permissions.CreditCardEdit:
				//case Permissions.Advertising:
				//case Permissions.RxMerge:
				case EnumPermType.MedicationDefEdit:
				case EnumPermType.AllergyDefEdit:
				//case Permissions.TextMessageView:
				case EnumPermType.TextMessageSend:
				//case Permissions.DefEdit:
				//case Permissions.UpdateInstall;
				case EnumPermType.AdjustmentTypeDeny:
				//case Permissions.StatementCSV:
				//case Permissions.SecurityGlobal:
				//case Permissions.TaskDelete:
				case EnumPermType.SetupWizard:
				//case Permissions.ShowFeatures:
				//case Permissions.PrinterSetup:
				//case Permissions.ProviderAdd:
				//case Permissions.ClinicEdit:
				case EnumPermType.ApiAccountEdit:
				//case Permissions.RegistrationKeyCreate:
				//case Permissions.RegistrationKeyEdit:
				//case Permissions.AppointmentDelete:
				//case Permissions.AppointmentCompleteDelete:
				//case Permissions.AppointmentTypeEdit:
				//case Permissions.WebChatEdit:
				case EnumPermType.SupplierEdit:
				//case Permissions.SupplyPurchases:
				//case Permissions.PreferenceEditBroadcastMonitor:
				case EnumPermType.AppointmentResize:
				//case Permissions.CreditCardTerminal:
				case EnumPermType.ViewAppointmentAuditTrail:
				//case Permissions.PayPlanChargeEdit:
				case EnumPermType.ArchivedPatientSelect:
				case EnumPermType.CloudCustomerEdit:
				return false;//Does not have audit Trail if uncommented.
			}
			if(!PrefC.IsODHQ && permType.In(
					//These permissions are only used at OD HQ
					EnumPermType.VerifyPhoneOwnership,
					EnumPermType.HeadmasterSetup,
					EnumPermType.FAQEdit,
					EnumPermType.EditReadOnlyTasks,
					EnumPermType.TextingAccountEdit,
					EnumPermType.PreferenceEditBroadcastMonitor,
					EnumPermType.CloudCustomerEdit
				)) 
			{
				return false;
			}
			return true;
		}		

		///<summary>Removes all FKey specific permissions and gives the user group a single 'zero FKey' permission for the type passed in.</summary>
		public static void GiveUserGroupPermissionAll(long userGroupNum,EnumPermType permType) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),userGroupNum,permType);
				return;
			}
			//Remove all permissions for the user group and perm type.
			string command=$"DELETE FROM grouppermission WHERE UserGroupNum={POut.Long(userGroupNum)} AND PermType={POut.Enum(permType)}";
			Db.NonQ(command);
			//AdjustmentTypeDeny is a permission that denies access to a usergroup when they have this permission. When a user clicks 'Set All', they want the user group to have every permission.
			//This means they want the user group to have access to every adjustment type. So we need to delete all adjustment type deny permissions for this user group, which we do above. 
			//But we do NOT want to create a 0 FKey perm because that will indicate the user group does not have access to any adjusment type, so we return early.
			if(permType==EnumPermType.AdjustmentTypeDeny) {
				return;
			}
			//Insert a new permission with a zero FKey.
			GroupPermission groupPermission=new GroupPermission();
			groupPermission.NewerDate=DateTime.MinValue;
			groupPermission.NewerDays=0;
			groupPermission.PermType=permType;
			groupPermission.UserGroupNum=userGroupNum;
			groupPermission.FKey=0;
			Crud.GroupPermissionCrud.Insert(groupPermission);
		}

		///<summary>Gets the description for the specified permisssion.  Already translated.</summary>
		public static string GetDesc(EnumPermType perm){
			//No need to check MiddleTierRole; no call to db.
			return Lans.g("enumPermissions",perm.GetDescription());//If Description attribute is not defined, will default to perm.ToString()
		}

		///<summary></summary>
		public static bool PermTakesDates(EnumPermType permType){
			//No need to check MiddleTierRole; no call to db.
			if(permType==EnumPermType.AccountingCreate//prevents backdating
				|| permType==EnumPermType.AccountingEdit
				|| permType==EnumPermType.AdjustmentCreate
				|| permType==EnumPermType.AdjustmentEdit
				|| permType==EnumPermType.ClaimDelete
				|| permType==EnumPermType.ClaimHistoryEdit
				|| permType==EnumPermType.ClaimProcReceivedEdit
				|| permType==EnumPermType.ClaimSentEdit
				|| permType==EnumPermType.CommlogEdit
				|| permType==EnumPermType.DepositSlips//prevents backdating
				|| permType==EnumPermType.EquipmentDelete
				|| permType==EnumPermType.ImageDelete
				|| permType==EnumPermType.InsPayEdit
				|| permType==EnumPermType.InsWriteOffEdit
				|| permType==EnumPermType.NewClaimsProcNotBilled
				|| permType==EnumPermType.OrthoChartEditFull
				|| permType==EnumPermType.OrthoChartEditUser
				|| permType==EnumPermType.PaymentEdit
				|| permType==EnumPermType.PerioEdit
				|| permType==EnumPermType.PreAuthSentEdit
				|| permType==EnumPermType.ProcComplCreate
				|| permType==EnumPermType.ProcCompleteEdit
				|| permType==EnumPermType.ProcCompleteNote
				|| permType==EnumPermType.ProcCompleteEditMisc
				|| permType==EnumPermType.ProcCompleteStatusEdit
				|| permType==EnumPermType.ProcCompleteAddAdj
				|| permType==EnumPermType.ProcExistingEdit
				|| permType==EnumPermType.ProcDelete
				|| permType==EnumPermType.SheetEdit
				|| permType==EnumPermType.TimecardDeleteEntry
				|| permType==EnumPermType.TreatPlanEdit
				|| permType==EnumPermType.TreatPlanSign
				|| permType==EnumPermType.PaymentCreate//to prevent backdating of newly created payments
				|| permType==EnumPermType.ImageEdit
				|| permType==EnumPermType.ImageExport
				)
			{
				return true;
			}
			return false;
		}

		///<summary>Returns a list of permissions that are included in the bitwise enum crudSLFKeyPerms passed in.
		///Used in DBM and the crud generator.  Needs to be updated every time a new CrudAuditPerm is added.</summary>
		public static List<EnumPermType> GetPermsFromCrudAuditPerm(CrudAuditPerm crudSLFKeyPerms) {
			List<EnumPermType> listPerms=new List<EnumPermType>();
			//No check for none.
			if(crudSLFKeyPerms.HasFlag(CrudAuditPerm.AppointmentCompleteEdit)) { //b01
				listPerms.Add(EnumPermType.AppointmentCompleteEdit);
			}
			if(crudSLFKeyPerms.HasFlag(CrudAuditPerm.AppointmentCreate)) { //b010
				listPerms.Add(EnumPermType.AppointmentCreate);
			}
			if(crudSLFKeyPerms.HasFlag(CrudAuditPerm.AppointmentEdit)) { //b0100
				listPerms.Add(EnumPermType.AppointmentEdit);
			}
			if(crudSLFKeyPerms.HasFlag(CrudAuditPerm.AppointmentMove)) { //b01000
				listPerms.Add(EnumPermType.AppointmentMove);
			}
			if(crudSLFKeyPerms.HasFlag(CrudAuditPerm.ClaimHistoryEdit)) { //b010000
				listPerms.Add(EnumPermType.ClaimHistoryEdit);
			}
			if(crudSLFKeyPerms.HasFlag(CrudAuditPerm.ImageDelete)) { //b0100000
				listPerms.Add(EnumPermType.ImageDelete);
			}
			if(crudSLFKeyPerms.HasFlag(CrudAuditPerm.ImageEdit)) { //b01000000
				listPerms.Add(EnumPermType.ImageEdit);
			}
			if(crudSLFKeyPerms.HasFlag(CrudAuditPerm.InsPlanChangeCarrierName)) { //b010000000
				listPerms.Add(EnumPermType.InsPlanChangeCarrierName);
			}
			if(crudSLFKeyPerms.HasFlag(CrudAuditPerm.RxCreate)) { //b0100000000
				listPerms.Add(EnumPermType.RxCreate);
			}
			if(crudSLFKeyPerms.HasFlag(CrudAuditPerm.RxEdit)) { //b01000000000
				listPerms.Add(EnumPermType.RxEdit);
			}
			if(crudSLFKeyPerms.HasFlag(CrudAuditPerm.TaskNoteEdit)) { //b010000000000
				listPerms.Add(EnumPermType.TaskNoteEdit);
			}
			if(crudSLFKeyPerms.HasFlag(CrudAuditPerm.PatientPortal)) { //b0100000000000
				listPerms.Add(EnumPermType.PatientPortal);
			}
			if(crudSLFKeyPerms.HasFlag(CrudAuditPerm.ProcFeeEdit)) { //b01000000000000
				listPerms.Add(EnumPermType.ProcFeeEdit);
			}
			if(crudSLFKeyPerms.HasFlag(CrudAuditPerm.LogFeeEdit)) { //b010000000000000
				listPerms.Add(EnumPermType.LogFeeEdit);
			}
			if(crudSLFKeyPerms.HasFlag(CrudAuditPerm.LogSubscriberEdit)) { //b0100000000000000
				listPerms.Add(EnumPermType.LogSubscriberEdit);
			}
			if(crudSLFKeyPerms.HasFlag(CrudAuditPerm.AppointmentDelete)) { //b01000000000000000
				listPerms.Add(EnumPermType.AppointmentDelete);
			}
			if(crudSLFKeyPerms.HasFlag(CrudAuditPerm.AppointmentCompleteDelete)) { //b010000000000000000
				listPerms.Add(EnumPermType.AppointmentCompleteDelete);
			}
			return listPerms;
		}
	}
 



}













