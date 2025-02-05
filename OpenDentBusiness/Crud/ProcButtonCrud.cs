//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace OpenDentBusiness.Crud{
	public class ProcButtonCrud {
		///<summary>Gets one ProcButton object from the database using the primary key.  Returns null if not found.</summary>
		public static ProcButton SelectOne(long procButtonNum) {
			string command="SELECT * FROM procbutton "
				+"WHERE ProcButtonNum = "+POut.Long(procButtonNum);
			List<ProcButton> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one ProcButton object from the database using a query.</summary>
		public static ProcButton SelectOne(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<ProcButton> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of ProcButton objects from the database using a query.</summary>
		public static List<ProcButton> SelectMany(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<ProcButton> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<ProcButton> TableToList(DataTable table) {
			List<ProcButton> retVal=new List<ProcButton>();
			ProcButton procButton;
			foreach(DataRow row in table.Rows) {
				procButton=new ProcButton();
				procButton.ProcButtonNum= PIn.Long  (row["ProcButtonNum"].ToString());
				procButton.Description  = PIn.String(row["Description"].ToString());
				procButton.ItemOrder    = PIn.Int   (row["ItemOrder"].ToString());
				procButton.Category     = PIn.Long  (row["Category"].ToString());
				procButton.ButtonImage  = PIn.String(row["ButtonImage"].ToString());
				procButton.IsMultiVisit = PIn.Bool  (row["IsMultiVisit"].ToString());
				retVal.Add(procButton);
			}
			return retVal;
		}

		///<summary>Converts a list of ProcButton into a DataTable.</summary>
		public static DataTable ListToTable(List<ProcButton> listProcButtons,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="ProcButton";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("ProcButtonNum");
			table.Columns.Add("Description");
			table.Columns.Add("ItemOrder");
			table.Columns.Add("Category");
			table.Columns.Add("ButtonImage");
			table.Columns.Add("IsMultiVisit");
			foreach(ProcButton procButton in listProcButtons) {
				table.Rows.Add(new object[] {
					POut.Long  (procButton.ProcButtonNum),
					            procButton.Description,
					POut.Int   (procButton.ItemOrder),
					POut.Long  (procButton.Category),
					            procButton.ButtonImage,
					POut.Bool  (procButton.IsMultiVisit),
				});
			}
			return table;
		}

		///<summary>Inserts one ProcButton into the database.  Returns the new priKey.</summary>
		public static long Insert(ProcButton procButton) {
			return Insert(procButton,false);
		}

		///<summary>Inserts one ProcButton into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(ProcButton procButton,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				procButton.ProcButtonNum=ReplicationServers.GetKey("procbutton","ProcButtonNum");
			}
			string command="INSERT INTO procbutton (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="ProcButtonNum,";
			}
			command+="Description,ItemOrder,Category,ButtonImage,IsMultiVisit) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(procButton.ProcButtonNum)+",";
			}
			command+=
				 "'"+POut.String(procButton.Description)+"',"
				+    POut.Int   (procButton.ItemOrder)+","
				+    POut.Long  (procButton.Category)+","
				+    DbHelper.ParamChar+"paramButtonImage,"
				+    POut.Bool  (procButton.IsMultiVisit)+")";
			if(procButton.ButtonImage==null) {
				procButton.ButtonImage="";
			}
			OdSqlParameter paramButtonImage=new OdSqlParameter("paramButtonImage",OdDbType.Text,POut.StringParam(procButton.ButtonImage));
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command,paramButtonImage);
			}
			else {
				procButton.ProcButtonNum=Db.NonQ(command,true,"ProcButtonNum","procButton",paramButtonImage);
			}
			return procButton.ProcButtonNum;
		}

		///<summary>Inserts one ProcButton into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(ProcButton procButton) {
			return InsertNoCache(procButton,false);
		}

		///<summary>Inserts one ProcButton into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(ProcButton procButton,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO procbutton (";
			if(!useExistingPK && isRandomKeys) {
				procButton.ProcButtonNum=ReplicationServers.GetKeyNoCache("procbutton","ProcButtonNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="ProcButtonNum,";
			}
			command+="Description,ItemOrder,Category,ButtonImage,IsMultiVisit) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(procButton.ProcButtonNum)+",";
			}
			command+=
				 "'"+POut.String(procButton.Description)+"',"
				+    POut.Int   (procButton.ItemOrder)+","
				+    POut.Long  (procButton.Category)+","
				+    DbHelper.ParamChar+"paramButtonImage,"
				+    POut.Bool  (procButton.IsMultiVisit)+")";
			if(procButton.ButtonImage==null) {
				procButton.ButtonImage="";
			}
			OdSqlParameter paramButtonImage=new OdSqlParameter("paramButtonImage",OdDbType.Text,POut.StringParam(procButton.ButtonImage));
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command,paramButtonImage);
			}
			else {
				procButton.ProcButtonNum=Db.NonQ(command,true,"ProcButtonNum","procButton",paramButtonImage);
			}
			return procButton.ProcButtonNum;
		}

		///<summary>Updates one ProcButton in the database.</summary>
		public static void Update(ProcButton procButton) {
			string command="UPDATE procbutton SET "
				+"Description  = '"+POut.String(procButton.Description)+"', "
				+"ItemOrder    =  "+POut.Int   (procButton.ItemOrder)+", "
				+"Category     =  "+POut.Long  (procButton.Category)+", "
				+"ButtonImage  =  "+DbHelper.ParamChar+"paramButtonImage, "
				+"IsMultiVisit =  "+POut.Bool  (procButton.IsMultiVisit)+" "
				+"WHERE ProcButtonNum = "+POut.Long(procButton.ProcButtonNum);
			if(procButton.ButtonImage==null) {
				procButton.ButtonImage="";
			}
			OdSqlParameter paramButtonImage=new OdSqlParameter("paramButtonImage",OdDbType.Text,POut.StringParam(procButton.ButtonImage));
			Db.NonQ(command,paramButtonImage);
		}

		///<summary>Updates one ProcButton in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(ProcButton procButton,ProcButton oldProcButton) {
			string command="";
			if(procButton.Description != oldProcButton.Description) {
				if(command!="") { command+=",";}
				command+="Description = '"+POut.String(procButton.Description)+"'";
			}
			if(procButton.ItemOrder != oldProcButton.ItemOrder) {
				if(command!="") { command+=",";}
				command+="ItemOrder = "+POut.Int(procButton.ItemOrder)+"";
			}
			if(procButton.Category != oldProcButton.Category) {
				if(command!="") { command+=",";}
				command+="Category = "+POut.Long(procButton.Category)+"";
			}
			if(procButton.ButtonImage != oldProcButton.ButtonImage) {
				if(command!="") { command+=",";}
				command+="ButtonImage = "+DbHelper.ParamChar+"paramButtonImage";
			}
			if(procButton.IsMultiVisit != oldProcButton.IsMultiVisit) {
				if(command!="") { command+=",";}
				command+="IsMultiVisit = "+POut.Bool(procButton.IsMultiVisit)+"";
			}
			if(command=="") {
				return false;
			}
			if(procButton.ButtonImage==null) {
				procButton.ButtonImage="";
			}
			OdSqlParameter paramButtonImage=new OdSqlParameter("paramButtonImage",OdDbType.Text,POut.StringParam(procButton.ButtonImage));
			command="UPDATE procbutton SET "+command
				+" WHERE ProcButtonNum = "+POut.Long(procButton.ProcButtonNum);
			Db.NonQ(command,paramButtonImage);
			return true;
		}

		///<summary>Returns true if Update(ProcButton,ProcButton) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(ProcButton procButton,ProcButton oldProcButton) {
			if(procButton.Description != oldProcButton.Description) {
				return true;
			}
			if(procButton.ItemOrder != oldProcButton.ItemOrder) {
				return true;
			}
			if(procButton.Category != oldProcButton.Category) {
				return true;
			}
			if(procButton.ButtonImage != oldProcButton.ButtonImage) {
				return true;
			}
			if(procButton.IsMultiVisit != oldProcButton.IsMultiVisit) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one ProcButton from the database.</summary>
		public static void Delete(long procButtonNum) {
			string command="DELETE FROM procbutton "
				+"WHERE ProcButtonNum = "+POut.Long(procButtonNum);
			Db.NonQ(command);
		}

		///<summary>Deletes many ProcButtons from the database.</summary>
		public static void DeleteMany(List<long> listProcButtonNums) {
			if(listProcButtonNums==null || listProcButtonNums.Count==0) {
				return;
			}
			string command="DELETE FROM procbutton "
				+"WHERE ProcButtonNum IN("+string.Join(",",listProcButtonNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

	}
}