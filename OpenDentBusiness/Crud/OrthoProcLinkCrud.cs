//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace OpenDentBusiness.Crud{
	public class OrthoProcLinkCrud {
		///<summary>Gets one OrthoProcLink object from the database using the primary key.  Returns null if not found.</summary>
		public static OrthoProcLink SelectOne(long orthoProcLinkNum) {
			string command="SELECT * FROM orthoproclink "
				+"WHERE OrthoProcLinkNum = "+POut.Long(orthoProcLinkNum);
			List<OrthoProcLink> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one OrthoProcLink object from the database using a query.</summary>
		public static OrthoProcLink SelectOne(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<OrthoProcLink> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of OrthoProcLink objects from the database using a query.</summary>
		public static List<OrthoProcLink> SelectMany(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<OrthoProcLink> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<OrthoProcLink> TableToList(DataTable table) {
			List<OrthoProcLink> retVal=new List<OrthoProcLink>();
			OrthoProcLink orthoProcLink;
			foreach(DataRow row in table.Rows) {
				orthoProcLink=new OrthoProcLink();
				orthoProcLink.OrthoProcLinkNum= PIn.Long  (row["OrthoProcLinkNum"].ToString());
				orthoProcLink.OrthoCaseNum    = PIn.Long  (row["OrthoCaseNum"].ToString());
				orthoProcLink.ProcNum         = PIn.Long  (row["ProcNum"].ToString());
				orthoProcLink.SecDateTEntry   = PIn.DateT (row["SecDateTEntry"].ToString());
				orthoProcLink.SecUserNumEntry = PIn.Long  (row["SecUserNumEntry"].ToString());
				orthoProcLink.ProcLinkType    = (OpenDentBusiness.OrthoProcType)PIn.Int(row["ProcLinkType"].ToString());
				retVal.Add(orthoProcLink);
			}
			return retVal;
		}

		///<summary>Converts a list of OrthoProcLink into a DataTable.</summary>
		public static DataTable ListToTable(List<OrthoProcLink> listOrthoProcLinks,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="OrthoProcLink";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("OrthoProcLinkNum");
			table.Columns.Add("OrthoCaseNum");
			table.Columns.Add("ProcNum");
			table.Columns.Add("SecDateTEntry");
			table.Columns.Add("SecUserNumEntry");
			table.Columns.Add("ProcLinkType");
			foreach(OrthoProcLink orthoProcLink in listOrthoProcLinks) {
				table.Rows.Add(new object[] {
					POut.Long  (orthoProcLink.OrthoProcLinkNum),
					POut.Long  (orthoProcLink.OrthoCaseNum),
					POut.Long  (orthoProcLink.ProcNum),
					POut.DateT (orthoProcLink.SecDateTEntry,false),
					POut.Long  (orthoProcLink.SecUserNumEntry),
					POut.Int   ((int)orthoProcLink.ProcLinkType),
				});
			}
			return table;
		}

		///<summary>Inserts one OrthoProcLink into the database.  Returns the new priKey.</summary>
		public static long Insert(OrthoProcLink orthoProcLink) {
			return Insert(orthoProcLink,false);
		}

		///<summary>Inserts one OrthoProcLink into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(OrthoProcLink orthoProcLink,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				orthoProcLink.OrthoProcLinkNum=ReplicationServers.GetKey("orthoproclink","OrthoProcLinkNum");
			}
			string command="INSERT INTO orthoproclink (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="OrthoProcLinkNum,";
			}
			command+="OrthoCaseNum,ProcNum,SecDateTEntry,SecUserNumEntry,ProcLinkType) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(orthoProcLink.OrthoProcLinkNum)+",";
			}
			command+=
				     POut.Long  (orthoProcLink.OrthoCaseNum)+","
				+    POut.Long  (orthoProcLink.ProcNum)+","
				+    DbHelper.Now()+","
				+    POut.Long  (orthoProcLink.SecUserNumEntry)+","
				+    POut.Int   ((int)orthoProcLink.ProcLinkType)+")";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				orthoProcLink.OrthoProcLinkNum=Db.NonQ(command,true,"OrthoProcLinkNum","orthoProcLink");
			}
			return orthoProcLink.OrthoProcLinkNum;
		}

		///<summary>Inserts one OrthoProcLink into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(OrthoProcLink orthoProcLink) {
			return InsertNoCache(orthoProcLink,false);
		}

		///<summary>Inserts one OrthoProcLink into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(OrthoProcLink orthoProcLink,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO orthoproclink (";
			if(!useExistingPK && isRandomKeys) {
				orthoProcLink.OrthoProcLinkNum=ReplicationServers.GetKeyNoCache("orthoproclink","OrthoProcLinkNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="OrthoProcLinkNum,";
			}
			command+="OrthoCaseNum,ProcNum,SecDateTEntry,SecUserNumEntry,ProcLinkType) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(orthoProcLink.OrthoProcLinkNum)+",";
			}
			command+=
				     POut.Long  (orthoProcLink.OrthoCaseNum)+","
				+    POut.Long  (orthoProcLink.ProcNum)+","
				+    DbHelper.Now()+","
				+    POut.Long  (orthoProcLink.SecUserNumEntry)+","
				+    POut.Int   ((int)orthoProcLink.ProcLinkType)+")";
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command);
			}
			else {
				orthoProcLink.OrthoProcLinkNum=Db.NonQ(command,true,"OrthoProcLinkNum","orthoProcLink");
			}
			return orthoProcLink.OrthoProcLinkNum;
		}

		///<summary>Updates one OrthoProcLink in the database.</summary>
		public static void Update(OrthoProcLink orthoProcLink) {
			string command="UPDATE orthoproclink SET "
				+"OrthoCaseNum    =  "+POut.Long  (orthoProcLink.OrthoCaseNum)+", "
				+"ProcNum         =  "+POut.Long  (orthoProcLink.ProcNum)+", "
				//SecDateTEntry not allowed to change
				+"SecUserNumEntry =  "+POut.Long  (orthoProcLink.SecUserNumEntry)+", "
				+"ProcLinkType    =  "+POut.Int   ((int)orthoProcLink.ProcLinkType)+" "
				+"WHERE OrthoProcLinkNum = "+POut.Long(orthoProcLink.OrthoProcLinkNum);
			Db.NonQ(command);
		}

		///<summary>Updates one OrthoProcLink in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(OrthoProcLink orthoProcLink,OrthoProcLink oldOrthoProcLink) {
			string command="";
			if(orthoProcLink.OrthoCaseNum != oldOrthoProcLink.OrthoCaseNum) {
				if(command!="") { command+=",";}
				command+="OrthoCaseNum = "+POut.Long(orthoProcLink.OrthoCaseNum)+"";
			}
			if(orthoProcLink.ProcNum != oldOrthoProcLink.ProcNum) {
				if(command!="") { command+=",";}
				command+="ProcNum = "+POut.Long(orthoProcLink.ProcNum)+"";
			}
			//SecDateTEntry not allowed to change
			if(orthoProcLink.SecUserNumEntry != oldOrthoProcLink.SecUserNumEntry) {
				if(command!="") { command+=",";}
				command+="SecUserNumEntry = "+POut.Long(orthoProcLink.SecUserNumEntry)+"";
			}
			if(orthoProcLink.ProcLinkType != oldOrthoProcLink.ProcLinkType) {
				if(command!="") { command+=",";}
				command+="ProcLinkType = "+POut.Int   ((int)orthoProcLink.ProcLinkType)+"";
			}
			if(command=="") {
				return false;
			}
			command="UPDATE orthoproclink SET "+command
				+" WHERE OrthoProcLinkNum = "+POut.Long(orthoProcLink.OrthoProcLinkNum);
			Db.NonQ(command);
			return true;
		}

		///<summary>Returns true if Update(OrthoProcLink,OrthoProcLink) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(OrthoProcLink orthoProcLink,OrthoProcLink oldOrthoProcLink) {
			if(orthoProcLink.OrthoCaseNum != oldOrthoProcLink.OrthoCaseNum) {
				return true;
			}
			if(orthoProcLink.ProcNum != oldOrthoProcLink.ProcNum) {
				return true;
			}
			//SecDateTEntry not allowed to change
			if(orthoProcLink.SecUserNumEntry != oldOrthoProcLink.SecUserNumEntry) {
				return true;
			}
			if(orthoProcLink.ProcLinkType != oldOrthoProcLink.ProcLinkType) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one OrthoProcLink from the database.</summary>
		public static void Delete(long orthoProcLinkNum) {
			string command="DELETE FROM orthoproclink "
				+"WHERE OrthoProcLinkNum = "+POut.Long(orthoProcLinkNum);
			Db.NonQ(command);
		}

		///<summary>Deletes many OrthoProcLinks from the database.</summary>
		public static void DeleteMany(List<long> listOrthoProcLinkNums) {
			if(listOrthoProcLinkNums==null || listOrthoProcLinkNums.Count==0) {
				return;
			}
			string command="DELETE FROM orthoproclink "
				+"WHERE OrthoProcLinkNum IN("+string.Join(",",listOrthoProcLinkNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

	}
}