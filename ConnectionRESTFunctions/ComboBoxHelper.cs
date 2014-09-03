using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Cisco.UnityConnection.RestFunctions
{
    
    /// <summary>
    /// Class that provides a method for populating a combo box with items from a list of objects from the SDK and fetching
    /// them back from the currently selected item.
    /// Any object that supports the IUnityDisplayInterface can be used, most of them in the SDK can be used.
    /// </summary>
    public static class ComboBoxHelper
    {
        private static BindingList<IUnityDisplayInterface> _bindingObjects;

        /// <summary>
        /// Provide a list (IEnumerable) - a generic list is fine - of objects such as UserBase, CallHandler, Contact, UserTemplate,
        /// CallHandlerTemplate, Tenant etc... they will be presented to the user in a combo box using their display name or 
        /// equivalent for selection.
        /// </summary>
        /// <param name="pComboBox">
        /// ComboBox control
        /// </param>
        /// <param name="pObjects">
        /// List of objects to include in the combobox
        /// </param>
        /// <returns>
        /// Instance of the WebCallResults class 
        /// </returns>
        public static WebCallResult FillDropDownWithObjects(ref ComboBox pComboBox, IEnumerable<IUnityDisplayInterface> pObjects)
        {
            WebCallResult res = new WebCallResult {Success = false};

            if (pComboBox == null)
            {
                res.ErrorText =
                    "Null parameter passed for pComboBoxControl in FillInDropDownWithTenants on SettingsAccessDatabaseFunctions";
                return res;
            }

            if (pObjects == null || !pObjects.Any())
            {
                res.ErrorText = "Empty list of items passed to FillDropDownWithObjects";
                return res;
            }

            pComboBox.Items.Clear();
            try
            {
                _bindingObjects = new BindingList<IUnityDisplayInterface>();

                foreach (var oObject in pObjects)
                {
                    _bindingObjects.Add(oObject);
                }

                pComboBox.ValueMember = null;
                pComboBox.DisplayMember = "SelectionDisplayString";
                pComboBox.DataSource = _bindingObjects;
                pComboBox.ResetBindings();
            }
            catch (Exception ex)
            {
                res.ErrorText = "Failed binding list to comboBox:" + ex;
                return res;
            }

            //force the first item in the list to be selected - we know there's at least one item in the list at
            //this point so this should be a safe operation
            if (pComboBox.Items.Count > 0)
            {
                pComboBox.SelectedIndex = 0;
            }
            res.Success = true;
            return res;
        }

        /// <summary>
        /// Fetch the currently selected object from the combo box.  The type of the object you pass in must match the 
        /// type you used to fill the comboBox up with or this will fail.  An instance of that object (the same one you 
        /// put in the list the user selected from) will be returned in the out parameter.
        /// </summary>
        /// <param name="pComboBox">
        /// Combo box to fetch from.
        /// </param>
        /// <param name="pObject">
        /// Instance of the selected object will be passed back on this out param.
        /// </param>
        /// <returns>
        /// True if the fetch goes through ok, false if there is an error.
        /// </returns>
        public static bool GetCurrentComboBoxSelection<T>(ComboBox pComboBox, out T pObject)
        {
            pObject = default(T);

            if (pComboBox == null)
            {
                return false;
            }
            
            if (pComboBox.SelectedIndex < 0)
            {
                return false;
            }

            try
            {
                pObject = (T)pComboBox.SelectedValue;
            }
            catch (Exception ex)
            {
                Console.WriteLine("(error) converting to target type in GetCurrentComboBoxSelection:"+ex);
                return false;
            }

            return true;
        }
    }

   
}
