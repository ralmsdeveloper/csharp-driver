﻿//
//  Copyright (C) 2017 DataStax, Inc.
//
//  Please see the license for details:
//  http://www.datastax.com/terms/datastax-dse-driver-license-terms
//


namespace Dse.SessionManagement
{
    internal interface IDseSessionManagerFactory
    {
        ISessionManager Create(IInternalDseCluster dseCluster, IInternalDseSession dseSession);
    }
}