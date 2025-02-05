//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace OpenDentBusiness.Crud{
	public class FamilyHealthCrud {
		///<summary>Gets one FamilyHealth object from the database using the primary key.  Returns null if not found.</summary>
		public static FamilyHealth SelectOne(long familyHealthNum) {
			string command="SELECT * FROM familyhealth "
				+"WHERE FamilyHealthNum = "+POut.Long(familyHealthNum);
			List<FamilyHealth> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one FamilyHealth object from the database using a query.</summary>
		public static FamilyHealth SelectOne(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<FamilyHealth> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of FamilyHealth objects from the database using a query.</summary>
		public static List<FamilyHealth> SelectMany(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<FamilyHealth> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<FamilyHealth> TableToList(DataTable table) {
			List<FamilyHealth> retVal=new List<FamilyHealth>();
			FamilyHealth familyHealth;
			foreach(DataRow row in table.Rows) {
				familyHealth=new FamilyHealth();
				familyHealth.FamilyHealthNum= PIn.Long  (row["FamilyHealthNum"].ToString());
				familyHealth.PatNum         = PIn.Long  (row["PatNum"].ToString());
				familyHealth.Relationship   = (OpenDentBusiness.FamilyRelationship)PIn.Int(row["Relationship"].ToString());
				familyHealth.DiseaseDefNum  = PIn.Long  (row["DiseaseDefNum"].ToString());
				familyHealth.PersonName     = PIn.String(row["PersonName"].ToString());
				retVal.Add(familyHealth);
			}
			return retVal;
		}

		///<summary>Converts a list of FamilyHealth into a DataTable.</summary>
		public static DataTable ListToTable(List<FamilyHealth> listFamilyHealths,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="FamilyHealth";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("FamilyHealthNum");
			table.Columns.Add("PatNum");
			table.Columns.Add("Relationship");
			table.Columns.Add("DiseaseDefNum");
			table.Columns.Add("PersonName");
			foreach(FamilyHealth familyHealth in listFamilyHealths) {
				table.Rows.Add(new object[] {
					POut.Long  (familyHealth.FamilyHealthNum),
					POut.Long  (familyHealth.PatNum),
					POut.Int   ((int)familyHealth.Relationship),
					POut.Long  (familyHealth.DiseaseDefNum),
					            familyHealth.PersonName,
				});
			}
			return table;
		}

		///<summary>Inserts one FamilyHealth into the database.  Returns the new priKey.</summary>
		public static long Insert(FamilyHealth familyHealth) {
			return Insert(familyHealth,false);
		}

		///<summary>Inserts one FamilyHealth into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(FamilyHealth familyHealth,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				familyHealth.FamilyHealthNum=ReplicationServers.GetKey("familyhealth","FamilyHealthNum");
			}
			string command="INSERT INTO familyhealth (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="FamilyHealthNum,";
			}
			command+="PatNum,Relationship,DiseaseDefNum,PersonName) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(familyHealth.FamilyHealthNum)+",";
			}
			command+=
				     POut.Long  (familyHealth.PatNum)+","
				+    POut.Int   ((int)familyHealth.Relationship)+","
				+    POut.Long  (familyHealth.DiseaseDefNum)+","
				+"'"+POut.String(familyHealth.PersonName)+"')";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				familyHealth.FamilyHealthNum=Db.NonQ(command,true,"FamilyHealthNum","familyHealth");
			}
			return familyHealth.FamilyHealthNum;
		}

		///<summary>Inserts one FamilyHealth into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(FamilyHealth familyHealth) {
			return InsertNoCache(familyHealth,false);
		}

		///<summary>Inserts one FamilyHealth into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(FamilyHealth familyHealth,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO familyhealth (";
			if(!useExistingPK && isRandomKeys) {
				familyHealth.FamilyHealthNum=ReplicationServers.GetKeyNoCache("familyhealth","FamilyHealthNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="FamilyHealthNum,";
			}
			command+="PatNum,Relationship,DiseaseDefNum,PersonName) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(familyHealth.FamilyHealthNum)+",";
			}
			command+=
				     POut.Long  (familyHealth.PatNum)+","
				+    POut.Int   ((int)familyHealth.Relationship)+","
				+    POut.Long  (familyHealth.DiseaseDefNum)+","
				+"'"+POut.String(familyHealth.PersonName)+"')";
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command);
			}
			else {
				familyHealth.FamilyHealthNum=Db.NonQ(command,true,"FamilyHealthNum","familyHealth");
			}
			return familyHealth.FamilyHealthNum;
		}

		///<summary>Updates one FamilyHealth in the database.</summary>
		public static void Update(FamilyHealth familyHealth) {
			string command="UPDATE familyhealth SET "
				+"PatNum         =  "+POut.Long  (familyHealth.PatNum)+", "
				+"Relationship   =  "+POut.Int   ((int)familyHealth.Relationship)+", "
				+"DiseaseDefNum  =  "+POut.Long  (familyHealth.DiseaseDefNum)+", "
				+"PersonName     = '"+POut.String(familyHealth.PersonName)+"' "
				+"WHERE FamilyHealthNum = "+POut.Long(familyHealth.FamilyHealthNum);
			Db.NonQ(command);
		}

		///<summary>Updates one FamilyHealth in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(FamilyHealth familyHealth,FamilyHealth oldFamilyHealth) {
			string command="";
			if(familyHealth.PatNum != oldFamilyHealth.PatNum) {
				if(command!="") { command+=",";}
				command+="PatNum = "+POut.Long(familyHealth.PatNum)+"";
			}
			if(familyHealth.Relationship != oldFamilyHealth.Relationship) {
				if(command!="") { command+=",";}
				command+="Relationship = "+POut.Int   ((int)familyHealth.Relationship)+"";
			}
			if(familyHealth.DiseaseDefNum != oldFamilyHealth.DiseaseDefNum) {
				if(command!="") { command+=",";}
				command+="DiseaseDefNum = "+POut.Long(familyHealth.DiseaseDefNum)+"";
			}
			if(familyHealth.PersonName != oldFamilyHealth.PersonName) {
				if(command!="") { command+=",";}
				command+="PersonName = '"+POut.String(familyHealth.PersonName)+"'";
			}
			if(command=="") {
				return false;
			}
			command="UPDATE familyhealth SET "+command
				+" WHERE FamilyHealthNum = "+POut.Long(familyHealth.FamilyHealthNum);
			Db.NonQ(command);
			return true;
		}

		///<summary>Returns true if Update(FamilyHealth,FamilyHealth) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(FamilyHealth familyHealth,FamilyHealth oldFamilyHealth) {
			if(familyHealth.PatNum != oldFamilyHealth.PatNum) {
				return true;
			}
			if(familyHealth.Relationship != oldFamilyHealth.Relationship) {
				return true;
			}
			if(familyHealth.DiseaseDefNum != oldFamilyHealth.DiseaseDefNum) {
				return true;
			}
			if(familyHealth.PersonName != oldFamilyHealth.PersonName) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one FamilyHealth from the database.</summary>
		public static void Delete(long familyHealthNum) {
			string command="DELETE FROM familyhealth "
				+"WHERE FamilyHealthNum = "+POut.Long(familyHealthNum);
			Db.NonQ(command);
		}

		///<summary>Deletes many FamilyHealths from the database.</summary>
		public static void DeleteMany(List<long> listFamilyHealthNums) {
			if(listFamilyHealthNums==null || listFamilyHealthNums.Count==0) {
				return;
			}
			string command="DELETE FROM familyhealth "
				+"WHERE FamilyHealthNum IN("+string.Join(",",listFamilyHealthNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

	}
}