using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ConnectCsharpToMysql;
using System.Collections;
using AX_CRT_Mage_Connector.MagentoConnectService;
using AX_CRT_MAge_Connector;
using DynamicsConnectivityValidator;
using static AX_CRT_MAge_Connector.UtilityFunctions;

namespace MNC_Product_Sync
{
    class AxtoMageItemCategories1
    {

        private DBConnect dbConnect;


        static void MainS(string[] args)
        {

           

            string token_id = null;

            DBConnect db = null;
            AX_CRT_Mage_Connector.MagentoConnectService.PortTypeClient mage_client = null;

            try
            {
                var crtManager = new DynamicsRuntimeManager();

                CombinedWriter writer;
                var oldOut = Console.Out;

                try
                {
                    writer = new CombinedWriter("./DynamicsConnectivityValidator.log", false, Encoding.Unicode, 0x400, Console.Out);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Cannot open DynamicsConnectivityValidator.log for writing");
                    Console.WriteLine(e.Message);
                    return;
                }
                Microsoft.Dynamics.Commerce.Runtime.PagedResult<Microsoft.Dynamics.Commerce.Runtime.DataModel.Category> axcategories = AX_CRT_MAge_Connector.UtilityFunctions.readAxCategories(crtManager, writer);
                

                mage_client = new AX_CRT_Mage_Connector.MagentoConnectService.PortTypeClient();
                
                token_id = mage_client.login("webserviceuser", "apikey");

                db = new DBConnect();

                Hashtable hsc = db.fetch_CategoryMapping();
                String FirstLevelCaterogy="";
                List<catalogCategoryEntity1> secondlevelcategories = new List<catalogCategoryEntity1>();
                List<catalogCategoryEntity> thirdlevelcategories = new List<catalogCategoryEntity>();
                String FourthLevelCaterogy="";
                for (int i = 0; i < axcategories.Results.Count; i++)
                {
                    if (axcategories.Results[i].ParentCategory == 0)
                    {
                        FirstLevelCaterogy = axcategories.Results[i].RecordId.ToString();
                        //create root category
                        string[] sortby_values;
                    string sortbyval = "name";
                    sortby_values = sortbyval.ToString().Split(',');
                    catalogCategoryEntityCreate cat = new catalogCategoryEntityCreate();
                    cat.description = axcategories.Results[i].Name;
                    cat.name = axcategories.Results[i].Name;

                    cat.available_sort_by = sortby_values;
                    cat.include_in_menu = 1;

                    cat.default_sort_by = "name";
                    cat.is_active = 1;
                    cat.include_in_menuSpecified = true;
                    cat.include_in_menu = 1;

                    cat.is_activeSpecified = true;
                    cat.is_active = 1;
                    int magRootCat = 1;
                    //if (hsc[axcategories.Results[i].Name] != null)
                    //{
                    //    magRootCat = axcategories.Results[i].ParentCategory];
                    // }

                   
                        try
                        {
                            if (!hsc.ContainsKey(axcategories.Results[i].RecordId.ToString()))
                            {
                                int id = mage_client.catalogCategoryCreate(token_id, 2, cat, null);
                                db.InsertProductCatMapping(axcategories.Results[i].RecordId.ToString(), id.ToString(), cat.name);
                               
                                db.InsertLog("Category", axcategories.Results[i].RecordId.ToString(), "Ax Category Code", "SUCCESS");

                            }
                        }
                        catch(Exception ex)
                        {

                        }
                    }

                }

                // Second level categories
                 hsc = db.fetch_CategoryMapping();
                for (int i = 0; i < axcategories.Results.Count; i++)
                {

                    if (axcategories.Results[i].ParentCategory.ToString().Equals(FirstLevelCaterogy))
                    {

                        catalogCategoryEntity1 cc = new catalogCategoryEntity1();
                        cc.recordId = axcategories.Results[i].RecordId;
                        cc.Name = axcategories.Results[i].Name;
                        cc.ParentId = (int)axcategories.Results[i].ParentCategory;
                        secondlevelcategories.Add(cc);
                    }


                        bool flag = false;
                    {

                        // string[] a = new string[hsc.Values.Count];
                        if (hsc.ContainsValue(axcategories.Results[i].Name.ToString()))
                        {
                            flag = true;
                        }

                        if (hsc.ContainsKey(axcategories.Results[i].RecordId.ToString()))
                        {
                            flag = true;
                        }


                    }
                     if (flag == false)

                       
                    {
                        
                        if (axcategories.Results[i].ParentCategory.ToString().Equals(FirstLevelCaterogy))
                        {
                           
                     
                        string[] sortby_values;
                        string sortbyval = "name";
                        sortby_values = sortbyval.ToString().Split(',');
                        catalogCategoryEntityCreate cat = new catalogCategoryEntityCreate();
                        cat.description = axcategories.Results[i].Name;
                        cat.name = axcategories.Results[i].Name;

                        cat.available_sort_by = sortby_values;
                        cat.include_in_menu = 1;

                        cat.default_sort_by = "name";
                        cat.is_active = 1;
                        cat.include_in_menuSpecified = true;
                        cat.include_in_menu = 1;

                        cat.is_activeSpecified = true;
                        cat.is_active = 1;
                        //int magRootCat = 1;
                        //if (hsc[axcategories.Results[i].Name] != null)
                        //{
                        //    magRootCat = axcategories.Results[i].ParentCategory];
                        // }
                         
                      
                         int id = mage_client.catalogCategoryCreate(token_id, (int)db.fetch_MageCategoryID(axcategories.Results[i].ParentCategory), cat, null);
                         db.InsertProductCatMapping(axcategories.Results[i].RecordId.ToString(), id.ToString(), cat.name);
                         db.InsertLog("Category", axcategories.Results[i].RecordId.ToString(), "Navision Category Code", "SUCCESS");
                         }
                    }

                }

                //third level category

                
                hsc = db.fetch_CategoryMapping();
                for (int i = 0; i < axcategories.Results.Count; i++)
                {
                    bool flag = false;
                    {

                        // string[] a = new string[hsc.Values.Count];
                        if (hsc.ContainsValue(axcategories.Results[i].Name.ToString()))
                        {
                            flag = true;
                        }

                        if (hsc.ContainsKey(axcategories.Results[i].RecordId.ToString()))
                        {
                            flag = true;
                        }


                    }
                    if (flag == false)


                    {

                        bool flg = false;

                        for (int k=0;k<secondlevelcategories.Count;k++)
                        {
                            if (!axcategories.Results[i].ParentCategory.ToString().Equals(secondlevelcategories[k].recordId.ToString()))
                            {
                                continue;
                            }
                            else
                            {
                                flg = true;
                            }
                        }
                        if (flg)
                        {

                            catalogCategoryEntity cc = new catalogCategoryEntity();
                            cc.category_id = (int)axcategories.Results[i].RecordId;
                            cc.name = axcategories.Results[i].Name;
                            cc.parent_id = (int)axcategories.Results[i].ParentCategory;
                            thirdlevelcategories.Add(cc);
                            string[] sortby_values;
                            string sortbyval = "name";
                            sortby_values = sortbyval.ToString().Split(',');
                            catalogCategoryEntityCreate cat = new catalogCategoryEntityCreate();
                            cat.description = axcategories.Results[i].Name;
                            cat.name = axcategories.Results[i].Name;

                            cat.available_sort_by = sortby_values;
                            cat.include_in_menu = 1;

                            cat.default_sort_by = "name";
                            cat.is_active = 1;
                            cat.include_in_menuSpecified = true;
                            cat.include_in_menu = 1;

                            cat.is_activeSpecified = true;
                            cat.is_active = 1;
                            //int magRootCat = 1;
                            //if (hsc[axcategories.Results[i].Name] != null)
                            //{
                            //    magRootCat = axcategories.Results[i].ParentCategory];
                            // }


                            int id = mage_client.catalogCategoryCreate(token_id, (int)db.fetch_MageCategoryID(axcategories.Results[i].ParentCategory), cat, null);
                            db.InsertProductCatMapping(axcategories.Results[i].RecordId.ToString(), id.ToString(), cat.name);
                            db.InsertLog("Category", axcategories.Results[i].RecordId.ToString(), "Navision Category Code", "SUCCESS");
                        }
                    }

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
