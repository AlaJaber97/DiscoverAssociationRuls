﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace DiscoverAssociationRuls
{
    class Program
    {
        static List<Transaction> Transactions { get; set; }
        static TablesCandidate CandidateTableSizeOf { get; set; }
        static TablesFrequentItemSetSizeOf FrequentItemSetSizeOf { get; set; }
        static int MinimumSupport;
        static void Main()
        {
            try
            {
                Init();
                Console.Write("Plz. Enter Minimum Support:");
                MinimumSupport = int.Parse(Console.ReadLine());

                for(int i = 1; i < 7; i++)
                {
                    FrequentItemSetSizeOf.Add(GenarateCandidateTableSizeOfOf(i), MinimumSupport);
                    DisplayScreen(CandidateTableSizeOf[i], FrequentItemSetSizeOf[i]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static void Init()
        {
            Transactions = new List<Transaction>();
            CandidateTableSizeOf = new TablesCandidate();
            FrequentItemSetSizeOf = new TablesFrequentItemSetSizeOf();
            GetTranstactionDataFromExcelFile(Directory.GetCurrentDirectory() + "\\DataTest.xlsx");
        }
        public static void SearchInDataAbout(string ItemSet)
        {
            var Items = ItemSet.Split(',').Select(item=>item.Trim()).ToList<string>();
            var SizeItemSet = Items.Count();
            int frequentItem = 0;
            foreach (var Transaction in Transactions)
            {
                if(Items.All(item => Transaction.ItemSeparator.Contains(item)))
                    frequentItem += 1;
            }
            CandidateTableSizeOf[SizeItemSet].ListInstance.Find(x => x.ItemSet == ItemSet).SupportCount = frequentItem;
        }
        public static void GetTranstactionDataFromExcelFile(string filePath)
        {
            int IndexRow = 0;
            DataTable dtexcel = new DataTable();
            string strConn;
            if (filePath.Substring(filePath.LastIndexOf('.')).ToLower() == ".xlsx") //Excel 2007 or later
                strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=0\"";
            else //Excel  2003
                strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=0\"";
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();

            string query = "SELECT  * FROM [Sheet1$]";
            OleDbDataAdapter daexcel = new OleDbDataAdapter(query, conn);
            dtexcel.Locale = CultureInfo.CurrentCulture;
            daexcel.Fill(dtexcel);

            conn.Close();

            //Sort Data
            foreach (DataRow dataRow in dtexcel.Rows)
            {
                //Sparet Each Item
                var Data = dataRow.ItemArray[1].ToString().Split(',').AsQueryable<string>();
                //Trim Each Item
                Data = Data.AsQueryable<string>().Select(s => s.Trim().ToLower());
                //Order Item
                var ArrayOrder = Data.OrderBy(x => x);
                //Save ItemSet
                Transactions.Add(new Transaction(IndexRow.ToString(), string.Join(", ", ArrayOrder)));
                IndexRow++;
            }
            //return DataTable;

        }
        public static TableInstance GenarateCandidateTableSizeOfOf(int SizeCandidate)
        {
            TableInstance TableCandidate = new TableInstance();
            var ListItemSet = GenarateItemSetSizeOf(SizeCandidate);
            foreach (var ItemSet in ListItemSet)
            {
                TableCandidate.ListInstance.Add(new Instance(ItemSet, 0));
            }
            CandidateTableSizeOf.Add(TableCandidate);
            return TableCandidate;
        }
        public static List<string> GenarateItemSetSizeOf(int SizeItemSet)
        {
            if (SizeItemSet == 1)
                return GetItemsFormTransaction(Transactions);
            else
                return JoinItemSetFormPreviousCandidate(FrequentItemSetSizeOf[SizeItemSet-1].ListInstance.Select(item=>item.ItemSet).ToList<string>());
        }
        public static List<string> GetItemsFormTransaction(List<Transaction> TableTransaction)
        {
            var ListItems = new List<string>();
            foreach (var Transaction in TableTransaction)
            {
                foreach (var item in Transaction.ItemSeparator)
                {

                    if (!string.IsNullOrEmpty(item) && !ListItems.Exists(x => x == item))
                    {
                        ListItems.Add(item);
                    }
                }
            }
            ListItems = ListItems.OrderBy(item => item).ToList<string>();
            return ListItems;
        }
        public static List<string> JoinItemSetFormPreviousCandidate(List<string> ListItemSet)
        {
            if (ListItemSet.Count() == 0)
                throw new Exception("Can not Generate more Item Set");
            var SizeItemSet = ListItemSet[0].Split(',').Count();
            if (!ListItemSet.TrueForAll(item => item.Split(',').Count() == SizeItemSet))
                throw new Exception("Can not apply JoinItemSet on different item size");

            List<string> ItemSet = new List<string>();

            int NumberSteps = ListItemSet.Count() - 1;

            for (int Step = 0; Step < NumberSteps; Step++)
            {
                var MainItemSet = ListItemSet[0];
                ListItemSet.Remove(ListItemSet[0]);
                for (int Index = 0; Index < ListItemSet.Count(); Index++)
                {
                    string CurrentItemSet= ListItemSet[Index];

                    if (CanJoinThisTwoItems(MainItemSet, CurrentItemSet))
                    {
                        List<string> OrderArray = new List<string>();

                        OrderArray.AddRange(MainItemSet.Split(','));
                        OrderArray.AddRange(CurrentItemSet.Split(','));
                        OrderArray = OrderArray.Distinct().Select(s => s.Trim()).Select(s => s.ToLower()).ToList<string>();
                        OrderArray.Sort();

                        var newItem = string.Join(", ", OrderArray);
                        if (!ItemSet.Exists(item => item.Equals(newItem)))
                            ItemSet.Add(newItem);
                    }
                    else
                        break;
                }
            }
            return ItemSet;
        }
        public static bool CanJoinThisTwoItems(string Fstring, string Sstring)
        {
            int LastCommonMainItem = Fstring.LastIndexOf(',');
            int LastCommonCurrentItem = Sstring.LastIndexOf(',');

            var IsSignalItemSet = LastCommonMainItem == -1 || LastCommonCurrentItem == -1;
            if (IsSignalItemSet)
                return true;

            var IsSameFirstItems = Fstring.Substring(0, LastCommonMainItem) == Sstring.Substring(0, LastCommonCurrentItem);
            var IsDiffrentLastItem = Fstring.Substring(LastCommonMainItem, Fstring.Length - Fstring.Substring(0, LastCommonMainItem).Length) != Sstring.Substring(LastCommonCurrentItem, Sstring.Length - Sstring.Substring(0, LastCommonCurrentItem).Length);
            if (IsSameFirstItems && IsDiffrentLastItem)
                return true;

            return false;
        }
        public static void DisplayScreen(TableInstance CandidateTableSizeOf, TableInstance FrequentItemSetSizeOfTable)
        {
            Console.Write("\n\n\n\n");
            List<string> DisplayString = new List<string>();
            int SizeCell = 22;
            int SpaceBetweenTable = 20;

            string TopHeaderTable = $"|{FillSpace("Candidate Table", ' ', SizeCell*2+1)}|";
            PaddingRight(ref TopHeaderTable, ' ', SpaceBetweenTable);
            TopHeaderTable += $"|{ FillSpace("Frequent ItemSet Table", ' ', SizeCell*2+1)}| ";

            string TopSurface = string.Empty;
            PaddingRight(ref TopSurface, '-', SizeCell*2+3);
            PaddingRight(ref TopSurface, ' ', SpaceBetweenTable);
            TopSurface += TopSurface.Substring(0, TopSurface.Length- SpaceBetweenTable);

            string HeaderTable = $"|{FillSpace("ItemSet", ' ', SizeCell)}|{FillSpace("Support", ' ', SizeCell)}|";
            PaddingRight(ref HeaderTable, ' ', SpaceBetweenTable);
            HeaderTable += HeaderTable.Substring(0, HeaderTable.Length - SpaceBetweenTable);

            DisplayString.Add(TopHeaderTable);
            DisplayString.Add(TopSurface);
            DisplayString.Add(HeaderTable);
            DisplayString.Add(TopSurface);
            if (CandidateTableSizeOf.ListInstance.Count() != 0)
                for (int i = 0; i < CandidateTableSizeOf.ListInstance.Count(); i++)
                {
                    string ValueRow = $"|{FillSpace(CandidateTableSizeOf.ListInstance[i].ItemSet, ' ', SizeCell)}|{FillSpace(CandidateTableSizeOf.ListInstance[i].SupportCount.ToString(), ' ', SizeCell)}|";
                    PaddingRight(ref ValueRow, ' ', SpaceBetweenTable);
                    if (i < FrequentItemSetSizeOfTable.ListInstance.Count())
                        ValueRow += $"|{FillSpace(FrequentItemSetSizeOfTable.ListInstance[i].ItemSet, ' ', SizeCell)}|{FillSpace(FrequentItemSetSizeOfTable.ListInstance[i].SupportCount.ToString(), ' ', SizeCell)}|";
                    else
                        ValueRow += $"|{FillSpace("No Frequent ItemSet", ' ', SizeCell)}|{FillSpace("No Frequent ItemSet", ' ', SizeCell)}|";
                    DisplayString.Add(ValueRow);
                    DisplayString.Add(TopSurface);
                }
            else
            {
                string ValueRow = $"|{FillSpace("Can not Generate more", ' ', SizeCell)}|{FillSpace("Can not Generate more", ' ', SizeCell)}|";

                DisplayString.Add(ValueRow);
                DisplayString.Add(TopSurface);
            }

            foreach (var Line in DisplayString)
            {
                Console.WriteLine(Line);
            }
        }
        public static void PaddingRight(ref string S, char c, int n)
        {
            if (n > 0)
                while (n-- != 0)
                    S += c;
        }
        public static string FillSpace(string S, char c, int n)
        {
            string Temp = string.Empty;
            PaddingRight(ref Temp, c, (int)((n - S.Length) / 2.0 + 0.5));
            Temp += S;
            PaddingRight(ref Temp, c, (n - S.Length) / 2);
            return Temp;
        }
    }
}