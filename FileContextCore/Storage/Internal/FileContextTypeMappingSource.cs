﻿// Copyright (c) morrisjdev. All rights reserved.
// Original copyright (c) .NET Foundation. All rights reserved.
// Modified version by morrisjdev
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace FileContextCore.Storage.Internal
{
    /// <summary>
    ///     <para>
    ///         This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///         the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///         any release. You should only use it directly in your code with extreme caution and knowing that
    ///         doing so can result in application failures when updating to a new Entity Framework Core release.
    ///     </para>
    ///     <para>
    ///         The service lifetime is <see cref="ServiceLifetime.Singleton"/>. This means a single instance
    ///         is used by many <see cref="DbContext"/> instances. The implementation must be thread-safe.
    ///         This service cannot depend on services registered as <see cref="ServiceLifetime.Scoped"/>.
    ///     </para>
    /// </summary>
    public class FileContextTypeMappingSource : TypeMappingSource
    {
    
        public FileContextTypeMappingSource([NotNull] TypeMappingSourceDependencies dependencies)
            : base(dependencies)
        {
        }

    
        protected override CoreTypeMapping FindMapping(in TypeMappingInfo mappingInfo)
        {
            var clrType = mappingInfo.ClrType;
            Debug.Assert(clrType != null);

            if (clrType.IsValueType
                || clrType == typeof(string))
            {
                return new FileContextTypeMapping(clrType);
            }

            if (clrType == typeof(byte[]))
            {
                return new FileContextTypeMapping(clrType, structuralComparer: new ArrayStructuralComparer<byte>());
            }

            if (clrType.FullName == "NetTopologySuite.Geometries.Geometry"
                || clrType.GetBaseTypes().Any(t => t.FullName == "NetTopologySuite.Geometries.Geometry"))
            {
                var comparer = (ValueComparer)Activator.CreateInstance(typeof(GeometryValueComparer<>).MakeGenericType(clrType));

                return new FileContextTypeMapping(
                    clrType,
                    comparer,
                    comparer,
                    comparer);
            }

            return base.FindMapping(mappingInfo);
        }
    }
}
