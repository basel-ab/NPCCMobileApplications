﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Interop;
using NPCCMobileApplications.Library;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace NPCCMobileApplications.Droid
{
    public class assignment_lists : SupportFragment
    {
        SwipeRefreshLayout _swipeRefresh;
        FrameLayout mFragmentContainer;
        AppCompatActivity act;
        List<Spools> lstObjs;
        private RecyclerView rv;
        private SpoolsCardViewAdapter adapter;
        private npcc_types.inf_assignment_type _assignment_Type;

        public assignment_lists ins { get; set; }

        public assignment_lists(npcc_types.inf_assignment_type assignment_Type)
        {
            _assignment_Type = assignment_Type;
            ins = this;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.assignment, container, false);

            mFragmentContainer = this.Activity.FindViewById<FrameLayout>(Resource.Id.fragmentContainer);
            act = (AppCompatActivity)this.Activity;

            _swipeRefresh = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swiperefresh);
            rv = view.FindViewById<RecyclerView>(Resource.Id.mRecylcerID);
            rv.SetLayoutManager(new GridLayoutManager(act, 2));
            rv.SetItemAnimator(new DefaultItemAnimator());

            _swipeRefresh.Refresh += _swipeRefresh_Refresh;

            return view;
        }
        
        void _swipeRefresh_Refresh(object sender, EventArgs e)
        {
            refresh_listAsync();
        }

        void refresh_listAsync()
        {
            DBRepository dBRepository = new DBRepository();
            dBRepository.CreateTable();
            Task.Run(async () => { 
                await dBRepository.RefreshSpoolAsync();
            }).ContinueWith(fn => {
                act.RunOnUiThread(() => {
                    lstObjs = dBRepository.GetSpools();
                    adapter = new SpoolsCardViewAdapter(act, this, lstObjs);
                    rv.SetAdapter(adapter);
                    _swipeRefresh.Refreshing = false;
                });
            });
        }

        public void fill_listAsync()
        {
            if(rv.GetAdapter() == null)
            {
                DBRepository dBRepository = new DBRepository();
                dBRepository.CreateTable();
                lstObjs = dBRepository.GetSpools();
                if (lstObjs.Count == 0)
                {
                    _swipeRefresh.Refreshing = true;
                    refresh_listAsync();
                }
                else
                {
                    adapter = new SpoolsCardViewAdapter(act, this, lstObjs);
                    rv.SetAdapter(adapter);

                    _swipeRefresh.Refreshing = false;
                }
            }

        }
    }
}