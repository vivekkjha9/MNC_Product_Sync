using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ConnectCsharpToMysql;
using System.Collections;
using AX_CRT_Mage_Connector.MagentoConnectService;

namespace MNC_Product_Sync
{
    class AxtoMageItemCategories
    {

        private DBConnect dbConnect;


        static void MainCat(string[] args)
        {

           

            string token_id = null;

            DBConnect db = null;
            AX_CRT_Mage_Connector.MagentoConnectService.PortTypeClient mage_client = null;

            try
            {
                nv_ItemCats = client.ReadMultiple(null, null, 5000);

                mage_client = new AX_CRT_Mage_Connector.MagentoConnectService.PortTypeClient();
                
                token_id = mage_client.login("webserviceuser", "apikey");

                db = new DBConnect();
            }
            catch (Exception ex)
            {
                

            }
            finally
            {
                // mage_client.endSession(token_id);
            }
            try
            {
                for (int i = 0; i < nv_ItemCats.Length; i++)
                {




                    string[] sortby_values;
                    string sortbyval = "name";
                    sortby_values = sortbyval.ToString().Split(',');
                    catalogCategoryEntityCreate cat = new catalogCategoryEntityCreate();
                    cat.description = nv_ItemCats[i].Description;
                    cat.name = nv_ItemCats[i].Code;

                    cat.available_sort_by = sortby_values;
                    cat.include_in_menu = 1;

                    cat.default_sort_by = "name";
                    cat.is_active = 1;
                    cat.include_in_menuSpecified = true;
                    cat.include_in_menu = 1;

                    cat.is_activeSpecified = true;
                    cat.is_active = 1;


                    int id = mage_client.catalogCategoryCreate(token_id, 1, cat, null);
                    db.InsertProductCatMapping(nv_ItemCats[i].Code, id.ToString(), cat.name);
                    db.InsertLog("Category", nv_ItemCats[i].Code, "Navision Category Code", "SUCCESS");

                }



            }

            catch (Exception ex)
            {
               

       

            }
            finally
            {
                mage_client.endSession(token_id);
            }
            //mage_client.catalogProductCreate(token_id,



        }




           }
}
