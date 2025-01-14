//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace OpenDentBusiness.Crud{
	public class OrthoChartTabLinkCrud {
		///<summary>Gets one OrthoChartTabLink object from the database using the primary key.  Returns null if not found.</summary>
		public static OrthoChartTabLink SelectOne(long orthoChartTabLinkNum) {
			string command="SELECT * FROM orthocharttablink "
				+"WHERE OrthoChartTabLinkNum = "+POut.Long(orthoChartTabLinkNum);
			List<OrthoChartTabLink> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one OrthoChartTabLink object from the database using a query.</summary>
		public static OrthoChartTabLink SelectOne(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<OrthoChartTabLink> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of OrthoChartTabLink objects from the database using a query.</summary>
		public static List<OrthoChartTabLink> SelectMany(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<OrthoChartTabLink> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<OrthoChartTabLink> TableToList(DataTable table) {
			List<OrthoChartTabLink> retVal=new List<OrthoChartTabLink>();
			OrthoChartTabLink orthoChartTabLink;
			foreach(DataRow row in table.Rows) {
				orthoChartTabLink=new OrthoChartTabLink();
				orthoChartTabLink.OrthoChartTabLinkNum= PIn.Long  (row["OrthoChartTabLinkNum"].ToString());
				orthoChartTabLink.ItemOrder           = PIn.Int   (row["ItemOrder"].ToString());
				orthoChartTabLink.OrthoChartTabNum    = PIn.Long  (row["OrthoChartTabNum"].ToString());
				orthoChartTabLink.DisplayFieldNum     = PIn.Long  (row["DisplayFieldNum"].ToString());
				orthoChartTabLink.ColumnWidthOverride = PIn.Int   (row["ColumnWidthOverride"].ToString());
				retVal.Add(orthoChartTabLink);
			}
			return retVal;
		}

		///<summary>Converts a list of OrthoChartTabLink into a DataTable.</summary>
		public static DataTable ListToTable(List<OrthoChartTabLink> listOrthoChartTabLinks,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="OrthoChartTabLink";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("OrthoChartTabLinkNum");
			table.Columns.Add("ItemOrder");
			table.Columns.Add("OrthoChartTabNum");
			table.Columns.Add("DisplayFieldNum");
			table.Columns.Add("ColumnWidthOverride");
			foreach(OrthoChartTabLink orthoChartTabLink in listOrthoChartTabLinks) {
				table.Rows.Add(new object[] {
					POut.Long  (orthoChartTabLink.OrthoChartTabLinkNum),
					POut.Int   (orthoChartTabLink.ItemOrder),
					POut.Long  (orthoChartTabLink.OrthoChartTabNum),
					POut.Long  (orthoChartTabLink.DisplayFieldNum),
					POut.Int   (orthoChartTabLink.ColumnWidthOverride),
				});
			}
			return table;
		}

		///<summary>Inserts one OrthoChartTabLink into the database.  Returns the new priKey.</summary>
		public static long Insert(OrthoChartTabLink orthoChartTabLink) {
			return Insert(orthoChartTabLink,false);
		}

		///<summary>Inserts one OrthoChartTabLink into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(OrthoChartTabLink orthoChartTabLink,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				orthoChartTabLink.OrthoChartTabLinkNum=ReplicationServers.GetKey("orthocharttablink","OrthoChartTabLinkNum");
			}
			string command="INSERT INTO orthocharttablink (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="OrthoChartTabLinkNum,";
			}
			command+="ItemOrder,OrthoChartTabNum,DisplayFieldNum,ColumnWidthOverride) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(orthoChartTabLink.OrthoChartTabLinkNum)+",";
			}
			command+=
				     POut.Int   (orthoChartTabLink.ItemOrder)+","
				+    POut.Long  (orthoChartTabLink.OrthoChartTabNum)+","
				+    POut.Long  (orthoChartTabLink.DisplayFieldNum)+","
				+    POut.Int   (orthoChartTabLink.ColumnWidthOverride)+")";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				orthoChartTabLink.OrthoChartTabLinkNum=Db.NonQ(command,true,"OrthoChartTabLinkNum","orthoChartTabLink");
			}
			return orthoChartTabLink.OrthoChartTabLinkNum;
		}

		///<summary>Inserts one OrthoChartTabLink into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(OrthoChartTabLink orthoChartTabLink) {
			return InsertNoCache(orthoChartTabLink,false);
		}

		///<summary>Inserts one OrthoChartTabLink into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(OrthoChartTabLink orthoChartTabLink,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO orthocharttablink (";
			if(!useExistingPK && isRandomKeys) {
				orthoChartTabLink.OrthoChartTabLinkNum=ReplicationServers.GetKeyNoCache("orthocharttablink","OrthoChartTabLinkNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="OrthoChartTabLinkNum,";
			}
			command+="ItemOrder,OrthoChartTabNum,DisplayFieldNum,ColumnWidthOverride) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(orthoChartTabLink.OrthoChartTabLinkNum)+",";
			}
			command+=
				     POut.Int   (orthoChartTabLink.ItemOrder)+","
				+    POut.Long  (orthoChartTabLink.OrthoChartTabNum)+","
				+    POut.Long  (orthoChartTabLink.DisplayFieldNum)+","
				+    POut.Int   (orthoChartTabLink.ColumnWidthOverride)+")";
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command);
			}
			else {
				orthoChartTabLink.OrthoChartTabLinkNum=Db.NonQ(command,true,"OrthoChartTabLinkNum","orthoChartTabLink");
			}
			return orthoChartTabLink.OrthoChartTabLinkNum;
		}

		///<summary>Updates one OrthoChartTabLink in the database.</summary>
		public static void Update(OrthoChartTabLink orthoChartTabLink) {
			string command="UPDATE orthocharttablink SET "
				+"ItemOrder           =  "+POut.Int   (orthoChartTabLink.ItemOrder)+", "
				+"OrthoChartTabNum    =  "+POut.Long  (orthoChartTabLink.OrthoChartTabNum)+", "
				+"DisplayFieldNum     =  "+POut.Long  (orthoChartTabLink.DisplayFieldNum)+", "
				+"ColumnWidthOverride =  "+POut.Int   (orthoChartTabLink.ColumnWidthOverride)+" "
				+"WHERE OrthoChartTabLinkNum = "+POut.Long(orthoChartTabLink.OrthoChartTabLinkNum);
			Db.NonQ(command);
		}

		///<summary>Updates one OrthoChartTabLink in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(OrthoChartTabLink orthoChartTabLink,OrthoChartTabLink oldOrthoChartTabLink) {
			string command="";
			if(orthoChartTabLink.ItemOrder != oldOrthoChartTabLink.ItemOrder) {
				if(command!="") { command+=",";}
				command+="ItemOrder = "+POut.Int(orthoChartTabLink.ItemOrder)+"";
			}
			if(orthoChartTabLink.OrthoChartTabNum != oldOrthoChartTabLink.OrthoChartTabNum) {
				if(command!="") { command+=",";}
				command+="OrthoChartTabNum = "+POut.Long(orthoChartTabLink.OrthoChartTabNum)+"";
			}
			if(orthoChartTabLink.DisplayFieldNum != oldOrthoChartTabLink.DisplayFieldNum) {
				if(command!="") { command+=",";}
				command+="DisplayFieldNum = "+POut.Long(orthoChartTabLink.DisplayFieldNum)+"";
			}
			if(orthoChartTabLink.ColumnWidthOverride != oldOrthoChartTabLink.ColumnWidthOverride) {
				if(command!="") { command+=",";}
				command+="ColumnWidthOverride = "+POut.Int(orthoChartTabLink.ColumnWidthOverride)+"";
			}
			if(command=="") {
				return false;
			}
			command="UPDATE orthocharttablink SET "+command
				+" WHERE OrthoChartTabLinkNum = "+POut.Long(orthoChartTabLink.OrthoChartTabLinkNum);
			Db.NonQ(command);
			return true;
		}

		///<summary>Returns true if Update(OrthoChartTabLink,OrthoChartTabLink) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(OrthoChartTabLink orthoChartTabLink,OrthoChartTabLink oldOrthoChartTabLink) {
			if(orthoChartTabLink.ItemOrder != oldOrthoChartTabLink.ItemOrder) {
				return true;
			}
			if(orthoChartTabLink.OrthoChartTabNum != oldOrthoChartTabLink.OrthoChartTabNum) {
				return true;
			}
			if(orthoChartTabLink.DisplayFieldNum != oldOrthoChartTabLink.DisplayFieldNum) {
				return true;
			}
			if(orthoChartTabLink.ColumnWidthOverride != oldOrthoChartTabLink.ColumnWidthOverride) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one OrthoChartTabLink from the database.</summary>
		public static void Delete(long orthoChartTabLinkNum) {
			string command="DELETE FROM orthocharttablink "
				+"WHERE OrthoChartTabLinkNum = "+POut.Long(orthoChartTabLinkNum);
			Db.NonQ(command);
		}

		///<summary>Deletes many OrthoChartTabLinks from the database.</summary>
		public static void DeleteMany(List<long> listOrthoChartTabLinkNums) {
			if(listOrthoChartTabLinkNums==null || listOrthoChartTabLinkNums.Count==0) {
				return;
			}
			string command="DELETE FROM orthocharttablink "
				+"WHERE OrthoChartTabLinkNum IN("+string.Join(",",listOrthoChartTabLinkNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

		///<summary>Inserts, updates, or deletes database rows to match supplied list.  Returns true if db changes were made.</summary>
		public static bool Sync(List<OrthoChartTabLink> listNew,List<OrthoChartTabLink> listDB) {
			//Adding items to lists changes the order of operation. All inserts are completed first, then updates, then deletes.
			List<OrthoChartTabLink> listIns    =new List<OrthoChartTabLink>();
			List<OrthoChartTabLink> listUpdNew =new List<OrthoChartTabLink>();
			List<OrthoChartTabLink> listUpdDB  =new List<OrthoChartTabLink>();
			List<OrthoChartTabLink> listDel    =new List<OrthoChartTabLink>();
			listNew.Sort((OrthoChartTabLink x,OrthoChartTabLink y) => { return x.OrthoChartTabLinkNum.CompareTo(y.OrthoChartTabLinkNum); });
			listDB.Sort((OrthoChartTabLink x,OrthoChartTabLink y) => { return x.OrthoChartTabLinkNum.CompareTo(y.OrthoChartTabLinkNum); });
			int idxNew=0;
			int idxDB=0;
			int rowsUpdatedCount=0;
			OrthoChartTabLink fieldNew;
			OrthoChartTabLink fieldDB;
			//Because both lists have been sorted using the same criteria, we can now walk each list to determine which list contians the next element.  The next element is determined by Primary Key.
			//If the New list contains the next item it will be inserted.  If the DB contains the next item, it will be deleted.  If both lists contain the next item, the item will be updated.
			while(idxNew<listNew.Count || idxDB<listDB.Count) {
				fieldNew=null;
				if(idxNew<listNew.Count) {
					fieldNew=listNew[idxNew];
				}
				fieldDB=null;
				if(idxDB<listDB.Count) {
					fieldDB=listDB[idxDB];
				}
				//begin compare
				if(fieldNew!=null && fieldDB==null) {//listNew has more items, listDB does not.
					listIns.Add(fieldNew);
					idxNew++;
					continue;
				}
				else if(fieldNew==null && fieldDB!=null) {//listDB has more items, listNew does not.
					listDel.Add(fieldDB);
					idxDB++;
					continue;
				}
				else if(fieldNew.OrthoChartTabLinkNum<fieldDB.OrthoChartTabLinkNum) {//newPK less than dbPK, newItem is 'next'
					listIns.Add(fieldNew);
					idxNew++;
					continue;
				}
				else if(fieldNew.OrthoChartTabLinkNum>fieldDB.OrthoChartTabLinkNum) {//dbPK less than newPK, dbItem is 'next'
					listDel.Add(fieldDB);
					idxDB++;
					continue;
				}
				//Both lists contain the 'next' item, update required
				listUpdNew.Add(fieldNew);
				listUpdDB.Add(fieldDB);
				idxNew++;
				idxDB++;
			}
			//Commit changes to DB
			for(int i=0;i<listIns.Count;i++) {
				Insert(listIns[i]);
			}
			for(int i=0;i<listUpdNew.Count;i++) {
				if(Update(listUpdNew[i],listUpdDB[i])) {
					rowsUpdatedCount++;
				}
			}
			DeleteMany(listDel.Select(x => x.OrthoChartTabLinkNum).ToList());
			if(rowsUpdatedCount>0 || listIns.Count>0 || listDel.Count>0) {
				return true;
			}
			return false;
		}

	}
}