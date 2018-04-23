// Developer Express Code Central Example:
// How to implement CRUD operations using XtraGrid and PLinqInstantFeedbackSource
// 
// This example demonstrates how to implement the Create, Update and Delete
// operations using PLinqInstantFeedbackSource.
// This example works with the
// standard SQL Northwind database.
// 
// You can find sample updates and versions for different programming languages here:
// http://www.devexpress.com/example=E4501

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.UserSkins;
using DevExpress.XtraEditors;
using DevExpress.Data.PLinq;
using System.Linq;
using System.Data.Linq;

namespace PLinqServerMode
{
    public partial class Form1 : XtraForm
    {
        public Form1()
        {
            InitializeComponent();           
            plIFS.GetEnumerable += new EventHandler<GetEnumerableEventArgs>(plIFS_GetEnumerable);
            gridView1.AsyncCompleted += new EventHandler(gridView1_AsyncCompleted);
            oldRowsCount = gridView1.DataRowCount;
            gridControl.DataSource = plIFS;
        }

        void plIFS_GetEnumerable(object sender, GetEnumerableEventArgs e)
        {
            e.Source = nwdContext.Customers;
        }

        void gridView1_AsyncCompleted(object sender, EventArgs e)
        {
            if (customerToEdit != null && gridView1.DataRowCount > oldRowsCount)
            {
                for (int i = 0; i < gridView1.DataRowCount; i++)
                {
                    if (customerToEdit.CustomerID == gridView1.GetRowCellValue(i, gridView1.Columns["CustomerID"]).ToString())
                    {
                        gridView1.FocusedRowHandle = i;
                        oldRowsCount = gridView1.DataRowCount;
                        break;
                    }
                }
            }
        }
        PLinqInstantFeedbackSource plIFS = new PLinqInstantFeedbackSource();
        NorthwindDataContext nwdContext = new NorthwindDataContext();
        Customer customerToEdit;
        int oldRowsCount = 0;
        EditForm f1;

        private void button1_Click(object sender, EventArgs e)
        {
            customerToEdit = CreateCustomer();
            oldRowsCount = gridView1.DataRowCount;
            EditCustomer(customerToEdit, "NewCustomer", CloseNewCustomerHandler);
        }
        private Customer CreateCustomer()
        {
            string idString;
            var newCustomer = new Customer();
            while (true)
            {
                idString = GenerateCustomerID();
                if (!String.IsNullOrEmpty(idString))
                {
                    newCustomer.CustomerID = idString;
                    break;
                }
            }
            return newCustomer;
        }

        private string GenerateCustomerID()
        {
            const int IDLength = 5;
            var result = String.Empty;
            var rnd = new Random();
            bool collisionFlag = false;
            for (var i = 0; i < IDLength; i++)
            {
                result += Convert.ToChar(rnd.Next(65, 90));
            }
            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                if (result == gridView1.GetRowCellValue(i, gridView1.Columns["CustomerID"]).ToString())
                {
                    collisionFlag = true;
                    break;
                }
            }
            if (collisionFlag)
            {
                return String.Empty;
            }
            else
                return result;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            var query = nwdContext.Customers.Where<Customer>(customer => customer.CustomerID == GetCustomerIDByRowHandle(gridView1.FocusedRowHandle));
            customerToEdit = query.ToList()[0];
            EditCustomer(customerToEdit, "EditInfo", CloseEditCustomerHandler);
        }
        private void EditCustomer(Customer customer, string windowTitle, FormClosingEventHandler closedDelegate)
        {
            f1 = new EditForm(customer) { Text = windowTitle };
            f1.FormClosing += closedDelegate;
            f1.ShowDialog();
        }
        private string GetCustomerIDByRowHandle(int rowHandle)
        {
            return (string)gridView1.GetRowCellValue(rowHandle, "CustomerID");
        }
        private void CloseEditCustomerHandler(object sender, EventArgs e)
        {
            if (((EditForm)sender).DialogResult == DialogResult.OK)
            {
                try
                {
                    nwdContext.SubmitChanges();
                    nwdContext.Refresh(RefreshMode.OverwriteCurrentValues, nwdContext.Customers);
                    plIFS.Refresh();

                }
                catch (Exception ex)
                {
                    HandleExcepton(ex);
                }
            }
            customerToEdit = null;
        }
        private void CloseNewCustomerHandler(object sender, FormClosingEventArgs e)
        {
            if (((EditForm)sender).DialogResult == DialogResult.OK)
            {
                try
                {
                    nwdContext.Customers.InsertOnSubmit(customerToEdit);
                    nwdContext.SubmitChanges();
                    nwdContext.Refresh(RefreshMode.OverwriteCurrentValues, nwdContext.Customers);
                    plIFS.Refresh();

                }
                catch (Exception ex)
                {
                    HandleExcepton(ex);
                }
            }
            else
            {
                customerToEdit = null;
            }
        }

        private void HandleExcepton(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DeleteCustomer(gridView1.FocusedRowHandle);
        }
        private void DeleteCustomer(int focusedRowHandle)
        {
            if (focusedRowHandle < 0)
                return;
            if (MessageBox.Show("Do you really want to delete the selected customer?", "Delete Customer", MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                return;
            }
            var query = nwdContext.Customers.Where<Customer>(customer => customer.CustomerID == GetCustomerIDByRowHandle(focusedRowHandle));
            nwdContext.Customers.DeleteOnSubmit(query.ToList()[0]);
            try
            {
                nwdContext.SubmitChanges();
                nwdContext.Refresh(RefreshMode.OverwriteCurrentValues, nwdContext.Customers);
            }
            catch (Exception ex)
            {
                HandleExcepton(ex);
            }
            plIFS.Refresh();
            customerToEdit = null;
        }
    } 
}