﻿// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
//                    Michael DeMond
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Core
{
	public sealed class AllInterfaces : ReferenceCacheBase<TypeInfo, IReadOnlyList<TypeInfo>>, IAllInterfaces
	{
		readonly Func<TypeInfo, IEnumerable<TypeInfo>> _selector;

		public static AllInterfaces Default { get; } = new AllInterfaces();

		AllInterfaces()
		{
			_selector = Yield;
		}

		IEnumerable<TypeInfo> Yield(TypeInfo parameter) =>
			parameter.Yield()
			         .Concat(parameter.ImplementedInterfaces.YieldMetadata().SelectMany(_selector))
			         .Where(x => x.IsInterface)
			         .Distinct();

		protected override IReadOnlyList<TypeInfo> Create(TypeInfo parameter) => Yield(parameter).AsReadOnly();
	}
}