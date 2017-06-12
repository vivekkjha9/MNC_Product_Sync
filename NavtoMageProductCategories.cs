using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MNC_Product_Sync.MagentoConnectService;
using ConnectCsharpToMysql;
using System.Collections;


namespace MNC_Product_Sync
{
    class NavtoMageProductCategories
    {

        private DBConnect dbConnect;

         
        static void MainCat(string[] args)
        {

            //AppConfigFileSettings.UpdateAppSettings("address", "http://103.238.216.254:7047/DynamicsNAV71/WS/CRONUS%20India%20Ltd/Page/ItemCategory");
            Navision_ItemCategory_Service.ItemCategories nv_Item_Cat = new Navision_ItemCategory_Service.ItemCategories();


            Navision_ItemCategory_Service.ItemCategories_PortClient client = new Navision_ItemCategory_Service.ItemCategories_PortClient();

            Navision_ItemCategory_Service.ItemCategories[] nv_ItemCats = null;


            client.ClientCredentials.Windows.ClientCredential.UserName = "Administrator";

            client.ClientCredentials.Windows.ClientCredential.Password = "itree@123";

            client.ClientCredentials.Windows.ClientCredential.Domain = "122.166.222.116";

            string token_id = null;

            DBConnect db = null;

            db = new DBConnect();

            Hashtable hscat = db.fetch_CategoryMapping();

            MagentoConnectService.PortTypeClient mage_client = null;

            try
            {
                nv_ItemCats = client.ReadMultiple(null, null, 5000);

                mage_client = new MagentoConnectService.PortTypeClient();

                token_id = mage_client.login("webserviceuser", "apikey");

               
            }
            catch (Exception ex)
            {
                MNC_Product_Sync.ErrorLog errLog = new MNC_Product_Sync.ErrorLog();

                errLog.LogError("C:\\MNC_Logs", "ProductCategory : " + ex.Message);

            }
            finally
            {
               // mage_client.endSession(token_id);
            }
             try
                {
            for (int i = 0; i < nv_ItemCats.Length; i++)
            {


                    if (!db.fetch_ProductCat(nv_ItemCats[i].Code))
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
             
               
               
            }
            
         catch (Exception ex)
                {
                    MNC_Product_Sync.ErrorLog errLog = new MNC_Product_Sync.ErrorLog();

                    errLog.LogError("C:\\MNC_Logs", "ProductCategory : " + ex.Message);

                }
             finally
                {
                    mage_client.endSession(token_id);
                }
            //mage_client.catalogProductCreate(token_id,



        }

        


        public filters addFilter(filters filtresIn, string key, string op, string value)
        {
            filters filtres = filtresIn;
            if (filtres == null)
                filtres = new filters();

            complexFilter compfiltres = new complexFilter();
            compfiltres.key = key;
            associativeEntity ass = new associativeEntity();
            ass.key = op;
            ass.value = value;
            compfiltres.value = ass;

            List<complexFilter> tmpLst;
            if (filtres.complex_filter != null)
                tmpLst = filtres.complex_filter.ToList();
            else tmpLst = new List<complexFilter>();

            tmpLst.Add(compfiltres);

            filtres.complex_filter = tmpLst.ToArray();

            return filtres;
        }
    }
}
