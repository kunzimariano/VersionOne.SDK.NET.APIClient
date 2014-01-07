﻿using System;
using NUnit.Framework;

namespace VersionOne.SDK.APIClient.Tests {
	[TestFixture]
	[Ignore("VersionOne server accessible through both proxy is required")]
	public class ProxyTester {
		private const string V1Path = "http://integsrv01.internal.corp/VersionOneTest/";
		private const string V1Username = "admin";
		private const string V1Password = "admin";

		private const string ProxyPath = "http://integvm01:3128";
		private const string ProxyUsername = "user1";
		private const string ProxyPassword = "user1";

		[Test]
		public void QueryProjects() {
			var proxyProvider = new ProxyProvider(new Uri(ProxyPath), ProxyUsername, ProxyPassword);
			
			IAPIConnector metaConnector = new VersionOneAPIConnector(V1Path + "meta.v1/", proxyProvider: proxyProvider)
				.WithVersionOneUsernameAndPassword(V1Username, V1Password);
			IMetaModel metaModel = new MetaModel(metaConnector);

			IAPIConnector dataConnector = new VersionOneAPIConnector(V1Path + "rest-1.v1/", proxyProvider: proxyProvider)
				.WithVersionOneUsernameAndPassword(V1Username, V1Password);
			IServices services = new Services(metaModel, dataConnector);

			var projectType = metaModel.GetAssetType("Scope");
			var scopeQuery = new Query(projectType);
			var result = services.Retrieve(scopeQuery);

			Assert.IsTrue(result.Assets.Count > 0);
		}
	}
}