﻿using System;

namespace ExternalPropertyAttributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class InputAxisAttribute : DrawerAttribute
	{
	}
}
