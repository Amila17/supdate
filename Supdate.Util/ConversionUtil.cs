using System;
using System.Collections.Generic;
using System.Data;
using Supdate.Model;

namespace Supdate.Util
{
  public class ConversionUtil
  {
    public static DataTable EntityDisplayOrderToDataTable(IList<EntityDisplayOrder> displayOrders)
    {
      // Create data table to insert items
      var orderData = new DataTable("Items");
      orderData.Columns.Add("Key", typeof(string));
      orderData.Columns.Add("Value", typeof(int));

      // Add data in the data table.
      foreach (var entry in displayOrders)
      {
        orderData.Rows.Add(entry.EntityId, entry.DisplayOrder);
      }

      return orderData;
    }

    public static DataTable IntArrayToDataTable(int[] data)
    {
      // Create data table to insert items
      var dataTable = new DataTable("Items");
      dataTable.Columns.Add("intVal", typeof(int));

      if (data != null)
      {
        // Add data in the data table.
        foreach (var i in data)
        {
          dataTable.Rows.Add(i);
        }
      }

      return dataTable;
    }

    public static string DateTimeToFriendly(DateTime? dateTime, string stringIfDateIsNull)
    {
      return dateTime != null ? DateTimeToFriendly(dateTime.Value) : stringIfDateIsNull;
    }

    public static string DateTimeToFriendly(DateTime dateTime)
    {
      return dateTime.ToString("dd MMM, HH:mm");
    }
    public static string CommentToHashString(Comment comment)
    {
      return StringUtil.GetHashString(
        string.Format("{0}{1}{2}",
        comment.AuthorName,
        comment.AuthorEmail,
        ConfigUtil.CommentsHashKey)
        );
    }
    public static string IntToExcelColumnName(int columnNumber)
    {
      int dividend = columnNumber + 1;
      string columnName = String.Empty;

      while (dividend > 0)
      {
        int modulo = (dividend - 1) % 26;
        columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
        dividend = (int)((dividend - modulo) / 26);
      }

      return columnName;
    }
  }
}
