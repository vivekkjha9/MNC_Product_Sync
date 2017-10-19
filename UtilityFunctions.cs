
using DynamicsConnectivityValidator;
using Microsoft.Dynamics.Commerce.Runtime.DataModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

        //public static Microsoft.Dynamics.Commerce.Runtime.PagedResult<Customer> readCategories(DynamicsRuntimeManager runtimeManager, CombinedWriter writer)
        //{

        //    var channelId = runtimeManager.CommerceRuntime.CurrentPrincipal.ChannelId;
        //    QueryResultSettings queryResultSettings = QueryResultSettings.AllRecords;
        //    queryResultSettings.Paging = new PagingInfo(10); 
        //    return runtimeManager.ChannelManager.GetCustomers(m_oEnum, SearchLocation.Local, queryResultSettings);

        //}

        //public static Microsoft.Dynamics.Commerce.Runtime.PagedResult<Customer> readProducts(DynamicsRuntimeManager runtimeManager, CombinedWriter writer)
        //{

        //    var channelId = runtimeManager.CommerceRuntime.CurrentPrincipal.ChannelId;
        //    QueryResultSettings queryResultSettings = QueryResultSettings.AllRecords;
        //    queryResultSettings.Paging = new PagingInfo(10);
        //    return runtimeManager.ChannelManager.GetCustomers(m_oEnum, SearchLocation.Local, queryResultSettings);

        //}



    }



    }
