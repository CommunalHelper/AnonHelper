using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Celeste.Mod.Anonhelper;
using Celeste.Mod;

namespace Celeste.Mod.Anonhelper
{
	public class AnonhelperSession : EverestModuleSession
	{
		public bool HasCloudDash
		{
			get;
			set;
		} = false;
		public bool HasCoreDash
		{
			get;
			set;
		} = false;

		public bool HasJellyDash
		{
			get;
			set;
		} = false;

		public bool HasFeatherDash
		{
			get;
			set;
		} = false;

		public bool HasBoosterDash
		{
			get;
			set;
		} = false;

		public bool HasSuperDash
		{
			get;
			set;
		} = false;
	}
}