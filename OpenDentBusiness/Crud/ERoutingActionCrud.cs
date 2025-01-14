//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace OpenDentBusiness.Crud{
	public class ERoutingActionCrud {
		///<summary>Gets one ERoutingAction object from the database using the primary key.  Returns null if not found.</summary>
		public static ERoutingAction SelectOne(long eRoutingActionNum) {
			string command="SELECT * FROM eroutingaction "
				+"WHERE ERoutingActionNum = "+POut.Long(eRoutingActionNum);
			List<ERoutingAction> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one ERoutingAction object from the database using a query.</summary>
		public static ERoutingAction SelectOne(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<ERoutingAction> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of ERoutingAction objects from the database using a query.</summary>
		public static List<ERoutingAction> SelectMany(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<ERoutingAction> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<ERoutingAction> TableToList(DataTable table) {
			List<ERoutingAction> retVal=new List<ERoutingAction>();
			ERoutingAction eRoutingAction;
			foreach(DataRow row in table.Rows) {
				eRoutingAction=new ERoutingAction();
				eRoutingAction.ERoutingActionNum = PIn.Long  (row["ERoutingActionNum"].ToString());
				eRoutingAction.ERoutingNum       = PIn.Long  (row["ERoutingNum"].ToString());
				eRoutingAction.ItemOrder         = PIn.Int   (row["ItemOrder"].ToString());
				eRoutingAction.ERoutingActionType= (OpenDentBusiness.EnumERoutingActionType)PIn.Int(row["ERoutingActionType"].ToString());
				eRoutingAction.UserNum           = PIn.Long  (row["UserNum"].ToString());
				eRoutingAction.IsComplete        = PIn.Bool  (row["IsComplete"].ToString());
				eRoutingAction.DateTimeComplete  = PIn.DateT (row["DateTimeComplete"].ToString());
				retVal.Add(eRoutingAction);
			}
			return retVal;
		}

		///<summary>Converts a list of ERoutingAction into a DataTable.</summary>
		public static DataTable ListToTable(List<ERoutingAction> listERoutingActions,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="ERoutingAction";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("ERoutingActionNum");
			table.Columns.Add("ERoutingNum");
			table.Columns.Add("ItemOrder");
			table.Columns.Add("ERoutingActionType");
			table.Columns.Add("UserNum");
			table.Columns.Add("IsComplete");
			table.Columns.Add("DateTimeComplete");
			foreach(ERoutingAction eRoutingAction in listERoutingActions) {
				table.Rows.Add(new object[] {
					POut.Long  (eRoutingAction.ERoutingActionNum),
					POut.Long  (eRoutingAction.ERoutingNum),
					POut.Int   (eRoutingAction.ItemOrder),
					POut.Int   ((int)eRoutingAction.ERoutingActionType),
					POut.Long  (eRoutingAction.UserNum),
					POut.Bool  (eRoutingAction.IsComplete),
					POut.DateT (eRoutingAction.DateTimeComplete,false),
				});
			}
			return table;
		}

		///<summary>Inserts one ERoutingAction into the database.  Returns the new priKey.</summary>
		public static long Insert(ERoutingAction eRoutingAction) {
			return Insert(eRoutingAction,false);
		}

		///<summary>Inserts one ERoutingAction into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(ERoutingAction eRoutingAction,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				eRoutingAction.ERoutingActionNum=ReplicationServers.GetKey("eroutingaction","ERoutingActionNum");
			}
			string command="INSERT INTO eroutingaction (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="ERoutingActionNum,";
			}
			command+="ERoutingNum,ItemOrder,ERoutingActionType,UserNum,IsComplete,DateTimeComplete) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(eRoutingAction.ERoutingActionNum)+",";
			}
			command+=
				     POut.Long  (eRoutingAction.ERoutingNum)+","
				+    POut.Int   (eRoutingAction.ItemOrder)+","
				+    POut.Int   ((int)eRoutingAction.ERoutingActionType)+","
				+    POut.Long  (eRoutingAction.UserNum)+","
				+    POut.Bool  (eRoutingAction.IsComplete)+","
				+    POut.DateT (eRoutingAction.DateTimeComplete)+")";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				eRoutingAction.ERoutingActionNum=Db.NonQ(command,true,"ERoutingActionNum","eRoutingAction");
			}
			return eRoutingAction.ERoutingActionNum;
		}

		///<summary>Inserts one ERoutingAction into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(ERoutingAction eRoutingAction) {
			return InsertNoCache(eRoutingAction,false);
		}

		///<summary>Inserts one ERoutingAction into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(ERoutingAction eRoutingAction,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO eroutingaction (";
			if(!useExistingPK && isRandomKeys) {
				eRoutingAction.ERoutingActionNum=ReplicationServers.GetKeyNoCache("eroutingaction","ERoutingActionNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="ERoutingActionNum,";
			}
			command+="ERoutingNum,ItemOrder,ERoutingActionType,UserNum,IsComplete,DateTimeComplete) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(eRoutingAction.ERoutingActionNum)+",";
			}
			command+=
				     POut.Long  (eRoutingAction.ERoutingNum)+","
				+    POut.Int   (eRoutingAction.ItemOrder)+","
				+    POut.Int   ((int)eRoutingAction.ERoutingActionType)+","
				+    POut.Long  (eRoutingAction.UserNum)+","
				+    POut.Bool  (eRoutingAction.IsComplete)+","
				+    POut.DateT (eRoutingAction.DateTimeComplete)+")";
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command);
			}
			else {
				eRoutingAction.ERoutingActionNum=Db.NonQ(command,true,"ERoutingActionNum","eRoutingAction");
			}
			return eRoutingAction.ERoutingActionNum;
		}

		///<summary>Updates one ERoutingAction in the database.</summary>
		public static void Update(ERoutingAction eRoutingAction) {
			string command="UPDATE eroutingaction SET "
				+"ERoutingNum       =  "+POut.Long  (eRoutingAction.ERoutingNum)+", "
				+"ItemOrder         =  "+POut.Int   (eRoutingAction.ItemOrder)+", "
				+"ERoutingActionType=  "+POut.Int   ((int)eRoutingAction.ERoutingActionType)+", "
				+"UserNum           =  "+POut.Long  (eRoutingAction.UserNum)+", "
				+"IsComplete        =  "+POut.Bool  (eRoutingAction.IsComplete)+", "
				+"DateTimeComplete  =  "+POut.DateT (eRoutingAction.DateTimeComplete)+" "
				+"WHERE ERoutingActionNum = "+POut.Long(eRoutingAction.ERoutingActionNum);
			Db.NonQ(command);
		}

		///<summary>Updates one ERoutingAction in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(ERoutingAction eRoutingAction,ERoutingAction oldERoutingAction) {
			string command="";
			if(eRoutingAction.ERoutingNum != oldERoutingAction.ERoutingNum) {
				if(command!="") { command+=",";}
				command+="ERoutingNum = "+POut.Long(eRoutingAction.ERoutingNum)+"";
			}
			if(eRoutingAction.ItemOrder != oldERoutingAction.ItemOrder) {
				if(command!="") { command+=",";}
				command+="ItemOrder = "+POut.Int(eRoutingAction.ItemOrder)+"";
			}
			if(eRoutingAction.ERoutingActionType != oldERoutingAction.ERoutingActionType) {
				if(command!="") { command+=",";}
				command+="ERoutingActionType = "+POut.Int   ((int)eRoutingAction.ERoutingActionType)+"";
			}
			if(eRoutingAction.UserNum != oldERoutingAction.UserNum) {
				if(command!="") { command+=",";}
				command+="UserNum = "+POut.Long(eRoutingAction.UserNum)+"";
			}
			if(eRoutingAction.IsComplete != oldERoutingAction.IsComplete) {
				if(command!="") { command+=",";}
				command+="IsComplete = "+POut.Bool(eRoutingAction.IsComplete)+"";
			}
			if(eRoutingAction.DateTimeComplete != oldERoutingAction.DateTimeComplete) {
				if(command!="") { command+=",";}
				command+="DateTimeComplete = "+POut.DateT(eRoutingAction.DateTimeComplete)+"";
			}
			if(command=="") {
				return false;
			}
			command="UPDATE eroutingaction SET "+command
				+" WHERE ERoutingActionNum = "+POut.Long(eRoutingAction.ERoutingActionNum);
			Db.NonQ(command);
			return true;
		}

		///<summary>Returns true if Update(ERoutingAction,ERoutingAction) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(ERoutingAction eRoutingAction,ERoutingAction oldERoutingAction) {
			if(eRoutingAction.ERoutingNum != oldERoutingAction.ERoutingNum) {
				return true;
			}
			if(eRoutingAction.ItemOrder != oldERoutingAction.ItemOrder) {
				return true;
			}
			if(eRoutingAction.ERoutingActionType != oldERoutingAction.ERoutingActionType) {
				return true;
			}
			if(eRoutingAction.UserNum != oldERoutingAction.UserNum) {
				return true;
			}
			if(eRoutingAction.IsComplete != oldERoutingAction.IsComplete) {
				return true;
			}
			if(eRoutingAction.DateTimeComplete != oldERoutingAction.DateTimeComplete) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one ERoutingAction from the database.</summary>
		public static void Delete(long eRoutingActionNum) {
			string command="DELETE FROM eroutingaction "
				+"WHERE ERoutingActionNum = "+POut.Long(eRoutingActionNum);
			Db.NonQ(command);
		}

		///<summary>Deletes many ERoutingActions from the database.</summary>
		public static void DeleteMany(List<long> listERoutingActionNums) {
			if(listERoutingActionNums==null || listERoutingActionNums.Count==0) {
				return;
			}
			string command="DELETE FROM eroutingaction "
				+"WHERE ERoutingActionNum IN("+string.Join(",",listERoutingActionNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

	}
}