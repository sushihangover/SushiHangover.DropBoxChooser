using System;
using System.Linq;
using Com.Dropbox.Chooser.Android;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Text.Method;
using Android.Util;

namespace Xamarin.DropBoxChooser.Example
{
	[Activity(Label = "DropBoxChooser", MainLauncher = true, Icon = "@drawable/ic_launcher")]
	public class MainActivity : Activity
	{
		const string APP_KEY = /* This is for you to fill in! */; // https://www.dropbox.com/developers/apps
		const int DBX_CHOOSER_REQUEST = 0; // You can change this if needed

		Button mChooserButton;
		DbxChooser mChooser;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_main);

			mChooser = new DbxChooser(APP_KEY);
			mChooserButton = FindViewById<Button>(Resource.Id.chooser_button);
			mChooserButton.Click += delegate
			{
				DbxChooser.ResultType resultType;
				switch (FindViewById<RadioGroup>(Resource.Id.link_type).CheckedRadioButtonId)
				{
					case Resource.Id.link_type_direct:
						resultType = DbxChooser.ResultType.DirectLink;
						break;
					case Resource.Id.link_type_content:
						resultType = DbxChooser.ResultType.FileContent; 
						break;
					case Resource.Id.link_type_preview:
						resultType = DbxChooser.ResultType.PreviewLink;
						break;
					default:
						throw new Exception("unexpected link type!!");
				}
 				mChooser.ForResultType(resultType).Launch(this, DBX_CHOOSER_REQUEST);
			};
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			if (requestCode == DBX_CHOOSER_REQUEST)
			{
				if (resultCode == Result.Ok)
				{
					var result = new DbxChooser.Result(data);
					Log.Debug("main", "Link to selected file: " + result.Link);

					ShowLink(Resource.Id.uri, result.Link);
					(FindViewById<TextView>(Resource.Id.filename)).SetText(result.Name, TextView.BufferType.Normal);
					(FindViewById<TextView>(Resource.Id.size)).SetText(result.Size.ToString(), TextView.BufferType.Normal);
					ShowLink(Resource.Id.icon, result.Icon);

					var thumbs = result.Thumbnails;
					ShowLink(Resource.Id.thumb64, thumbs.FirstOrDefault(x => x.Key == "64x64").Value);
					ShowLink(Resource.Id.thumb200, thumbs.FirstOrDefault(x => x.Key == "200x200").Value);
					ShowLink(Resource.Id.thumb640, thumbs.FirstOrDefault(x => x.Key == "640x480").Value);
				}
				else
				{
					// Failed or was cancelled by the user.
				}
			}
			else
			{
				base.OnActivityResult(requestCode, resultCode, data);
			}
		}

		protected void ShowLink(int id, global::Android.Net.Uri uri)
		{
			var v = FindViewById<TextView>(id);
			if (uri == null)
			{
				v.SetText("", TextView.BufferType.Normal);
				return;
			}
			v.SetText(uri.ToString(), TextView.BufferType.Normal);
			v.MovementMethod = LinkMovementMethod.Instance;
		}
	}
}


