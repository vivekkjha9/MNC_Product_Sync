
using DynamicsConnectivityValidator;
using Microsoft.Dynamics.Commerce.Runtime.DataModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AX_CRT_MAge_Connector
{
    static class UtilityFunctions
    {
        public static Microsoft.Dynamics.Commerce.Runtime.PagedResult<Customer> readCustomer(DynamicsRuntimeManager runtimeManager, CombinedWriter writer)
        {

           // var status = true;
            var channelId = runtimeManager.CommerceRuntime.CurrentPrincipal.ChannelId;
            QueryResultSettings queryResultSettings = QueryResultSettings.AllRecords;
            queryResultSettings.Paging = new PagingInfo(10);



            string sqlStr = "select accountnum from CUSTTABLE where DATAAREAID = 'usrt'";
            System.Data.DataTable results = new System.Data.DataTable();
            //Microsoft.Dynamics.Commerce.Runtime.Data.Types.DataTable results = new Microsoft.Dynamics.Commerce.Runtime.Data.Types.DataTable();
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection("Server=REP-DEV-1,1433;Database=AxDB;User Id=crtconnector;Password=abc123"))
            using (SqlCommand command = new SqlCommand(sqlStr, conn))
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                dataAdapter.Fill(results);
            StringBuilder CustomerAccountNumbers = new StringBuilder();
            for (int i = 0; i < results.Rows.Count; i++)
            {

                //CustomerAccountNumbers.Append("\"");
                CustomerAccountNumbers.Append(results.Rows[i]["accountnum"]);
                //CustomerAccountNumbers.Append("\"");
                if (i == results.Rows.Count - 1)
                {
                }
                else
                {
                    CustomerAccountNumbers.Append(",");
                }
                //Similarly for QuantityInIssueUnit_uom.
            }

             System.Collections.Generic.List<String> list = CustomerAccountNumbers.ToString().Split(',').ToList();
            //System.Collections.Generic.List<String> list = new List<string>{ "004046" };
            IEnumerable <string> m_oEnum = list;



            return runtimeManager.CustomerManager.GetCustomers(m_oEnum, SearchLocation.Local, queryResultSettings);
            

            
        }


        public static Microsoft.Dynamics.Commerce.Runtime.PagedResult<SalesOrder> createOrder1(DynamicsRuntimeManager runtimeManager, CombinedWriter writer, SimpleProduct simpleProduct)
        {

            // var status = true;
            var channelId = runtimeManager.CommerceRuntime.CurrentPrincipal.ChannelId;
            QueryResultSettings queryResultSettings = QueryResultSettings.AllRecords;
            queryResultSettings.Paging = new PagingInfo(10);

            runtimeManager.CreateCart(simpleProduct, runtimeManager);
            


            return null;

        }
        public static Microsoft.Dynamics.Commerce.Runtime.PagedResult<Category> readAxCategories(DynamicsRuntimeManager runtimeManager, CombinedWriter writer)
        {
            var status = true;
            var channelId = runtimeManager.CommerceRuntime.CurrentPrincipal.ChannelId;

            if (runtimeManager.ChannelManager == null)
            {
                writer.Write("Failed to create ChannelManager");
                return null;
            }

            var channelConfiguration = runtimeManager.ChannelManager.GetChannelConfiguration();
            if (channelConfiguration == null)
            {
                writer.Write(string.Format(CultureInfo.InvariantCulture, "Unable to get channel configuration for the channel {0}", channelId));
                //status = null;
            }
            else
            {
                writer.Write("ChannelCountryRegionISOCode = " + channelConfiguration.ChannelCountryRegionISOCode);
                writer.Write("Currency = " + channelConfiguration.Currency);
                writer.Write("CompanyCurrency = " + channelConfiguration.CompanyCurrency);
                writer.Write("CountryRegionId = " + channelConfiguration.CountryRegionId);
                writer.Write("DefaultLanguageId = " + channelConfiguration.DefaultLanguageId);
                writer.Write("TimeZoneInfoId = " + channelConfiguration.TimeZoneInfoId);
                writer.Write("GiftCardItemId = " + channelConfiguration.GiftCardItemId);
                runtimeManager.GiftCardItemId = channelConfiguration.GiftCardItemId;
            }
            QueryResultSettings queryResultSettings4 = QueryResultSettings.AllRecords;
            queryResultSettings4.Paging = new PagingInfo(50);
            var categories = runtimeManager.ChannelManager.GetChannelCategoryHierarchy(channelId, queryResultSettings4);
            if (categories == null || categories.Results.Count == 0)
            {
                writer.Write("No categories were found");
                status = false;
            }
            else
            {
                writer.Write(string.Format(CultureInfo.InvariantCulture, "{0} categories were found", categories.Results.Count));
            }

            return categories;
        }


        //public static Microsoft.Dynamics.Commerce.Runtime.PagedResult<Customer> readCategories(DynamicsRuntimeManager runtimeManager, CombinedWriter writer)
        //{

        //    var channelId = runtimeManager.CommerceRuntime.CurrentPrincipal.ChannelId;
        //    QueryResultSettings queryResultSettings = QueryResultSettings.AllRecords;
        //    queryResultSettings.Paging = new PagingInfo(10); 
        //    return runtimeManager.ChannelManager.GetCustomers(m_oEnum, SearchLocation.Local, queryResultSettings);

        //}

        /// <summary>
        /// icr
        /// </summary>
        /// <param name="runtimeManager"></param>
        /// <param name="writer"></param>
        /// <returns></returns>
        public static Microsoft.Dynamics.Commerce.Runtime.PagedResult<SimpleProduct> readProducts(DynamicsRuntimeManager runtimeManager, CombinedWriter writer,long catrecordId)
        {

            //    var channelId = runtimeManager.CommerceRuntime.CurrentPrincipal.ChannelId;
            //    QueryResultSettings queryResultSettings = QueryResultSettings.AllRecords;
            //    queryResultSettings.Paging = new PagingInfo(10);
            //    return runtimeManager.ChannelManager.GetCustomers(m_oEnum, SearchLocation.Local, queryResultSettings);
            var channelId = runtimeManager.CommerceRuntime.CurrentPrincipal.ChannelId;
            QueryResultSettings queryResultSettings = QueryResultSettings.AllRecords;
            queryResultSettings.Paging = new PagingInfo(500);
            var catalogs = runtimeManager.ProductManager.GetProductCatalogs(channelId, queryResultSettings);
            if (catalogs == null || catalogs.Results.Count == 0)
            {
                writer.Write("No catalogs were found");
               // status = false;
            }
            else
            {
                writer.Write(string.Format(CultureInfo.InvariantCulture, "{0} catalogs were found", catalogs.Results.Count));
            }

            QueryResultSettings queryResultSettings1 = QueryResultSettings.AllRecords;
            queryResultSettings1.Paging = new PagingInfo(1000);
           // queryResultSettings1.ChangeTracking = true;
            var products = runtimeManager.ProductManager.GetProducts(queryResultSettings1);
            var productsCatolog = runtimeManager.ProductManager.GetProductCatalogs(channelId, queryResultSettings1);

            var productsbyCat = runtimeManager.ProductManager.SearchByCategory(channelId, productsCatolog.Results[0].RecordId, catrecordId, "en-US", queryResultSettings1);

            StringBuilder productRecId = new StringBuilder();
            for (int k=0;k< productsbyCat.Results.Count;k++)
            {
                productRecId.Append(productsbyCat.Results[k].RecordId);
                productRecId.Append(",");
            }
            Microsoft.Dynamics.Commerce.Runtime.ColumnSet col= new Microsoft.Dynamics.Commerce.Runtime.ColumnSet() ;

            string[] list1 =  productRecId.ToString().Split(',');
            System.Collections.Generic.List<long> long_list1= new List<long>();
           // long[] long_list1 = new long[150];
            
            for(int z=0;z<list1.Length;z++)
            {
                if (list1[z] != "")
                {
                    long_list1.Add(long.Parse(list1[z]));
                }
            }
             

           // System.Collections.Generic.List<long> list = new List<long> { long_list1 };
            //System.Collections.Generic.List<String> list = new List<string>{ "004046" };
            IEnumerable<long> m_oEnum = long_list1;

            var products55 = runtimeManager.ProductManager.GetByIds(channelId, m_oEnum, queryResultSettings1);

            
                

         

            if (products == null || products.Results.Count == 0)
            {
                writer.Write("No products were found");
                //status = false;
            }
            return products55;

        }

        public class catalogCategoryEntity1
        {
           public long recordId;
           public long ParentId;
           public string Name;

        }

    }



}
