//
//  Copyright 2012  Xamarin Inc.
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.using System;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using FieldService.Android.Dialogs;
using FieldService.Data;
using FieldService.Utilities;
using FieldService.ViewModels;
using FieldService.Android.Utilities;

namespace FieldService.Android.Fragments {
    public class ExpenseFragment : Fragment {
        ListView expensesListView;
        ExpenseDialog expenseDialog;

        public List<Expense> Expenses
        {
            get;
            set;
        }

        /// <summary>
        /// selected assignment
        /// </summary>
        public Assignment Assignment
        {
            get;
            set;
        }

        /// <summary>
        /// expense view model from summary activity
        /// </summary>
        public ExpenseViewModel ExpenseViewModel
        {
            get;
            set;
        }

        public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView (inflater, container, savedInstanceState);
            var view = inflater.Inflate (Resource.Layout.ExpensesFragmentLayout, null, true);

            expensesListView = view.FindViewById<ListView> (Resource.Id.expenseListView);

            ReloadExpenses ();
            expensesListView.ItemClick += (sender, e) => {
                var textView = e.View.FindViewById<TextView> (Resource.Id.expenseText);

                var expense = Expenses.ElementAtOrDefault ((int)textView.Tag);

                expenseDialog = new ExpenseDialog(Activity);
                expenseDialog.Activity = Activity;
                expenseDialog.Assignment = Assignment;
                expenseDialog.CurrentExpense = expense;
                expenseDialog.ExpenseViewModel = ExpenseViewModel;
                expenseDialog.Show ();
            };

            return view;
        }


        /// <summary>
        /// Reload the view in the listview by itself without calling to reload the list.
        /// </summary>
        /// <param name="index">index of the list view item to reload</param>
        public void ReloadSingleListItem (int index)
        {
            if (expensesListView.FirstVisiblePosition < index && index < expensesListView.LastVisiblePosition) {
                var view = expensesListView.GetChildAt (index);
                if (view != null) {
                    expensesListView.Adapter.GetView (index, view, expensesListView);
                }
            }
        }

        /// <summary>
        /// Reloads the expense in the list view
        /// </summary>
        private void ReloadExpenses ()
        {
            if (Expenses != null) {
                expensesListView.Adapter = new ExpensesAdapter (Activity, Resource.Layout.ExpenseListItemLayout, Expenses);
            }
        }

        /// <summary>
        /// Reloads the expenses from the view model
        /// </summary>
        public void ReloadExpenseData()
        {
            ExpenseViewModel.LoadExpensesAsync (Assignment).ContinueOnUIThread (_ => {
                Expenses = ExpenseViewModel.Expenses;
                ReloadExpenses ();
                var items = Activity.FindViewById<TextView> (Resource.Id.selectedAssignmentTotalItems);
                items.Text = Assignment.TotalExpenses.ToString ("$0.00");
            });
        }
    }
}