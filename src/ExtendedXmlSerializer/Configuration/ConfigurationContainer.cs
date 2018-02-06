﻿// MIT License
// 
// Copyright (c) 2016-2018 Wojciech Nagórski
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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel;

namespace ExtendedXmlSerializer.Configuration
{
	public class ConfigurationContainer : ContextBase, IConfigurationContainer, IConfiguration
	{
		readonly IRootContext _context;
		readonly IServicesFactory _services;

		public ConfigurationContainer() : this(DefaultExtensions.Default.ToArray()) {}

		public ConfigurationContainer(params ISerializerExtension[] extensions) : this(new ExtensionCollection(extensions)) {}

		public ConfigurationContainer(IExtensionCollection extensions) : this(new RootContext(extensions)) {}

		public ConfigurationContainer(ITypeConfigurationContext parent) : this(parent.Root, parent) {}

		public ConfigurationContainer(IRootContext context) : this(context, context) {}

		public ConfigurationContainer(IRootContext context, IContext parent) :
			this(context, parent, ServicesFactory.Default) {}

		public ConfigurationContainer(IRootContext context, IContext parent, IServicesFactory services) : base(parent)
		{
			_context = context;
			_services = services;
		}

		public IConfigurationContainer Extend(ISerializerExtension extension)
		{
			var existing = _context.SingleOrDefault(extension.GetType()
			                                                 .GetTypeInfo()
			                                                 .IsInstanceOfType);
			if (existing != null)
			{
				_context.Remove(existing);
			}

			_context.Add(extension);
			return this;
		}

		public IEnumerator<ITypeConfiguration> GetEnumerator() => _context.Types.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public ISerializers Get()
		{
			using (var services = _services.Get(Root))
			{
				var result = services.Get<ISerializers>();
				return result;
			}
		}
	}
}