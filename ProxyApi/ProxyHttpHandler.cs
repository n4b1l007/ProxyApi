using System;
using System.ComponentModel.Composition;
using System.Web;

namespace ProxyApi
{
    /// <summary>
    /// An <see cref="IHttpHandler"/> implementation that returns a cached copy of the
    /// generated proxy script.
    /// </summary>
    [Export]
	public class ProxyHttpHandler : IHttpHandler
	{
		private static string _proxyJs;
		private static volatile object _syncRoot = new object();
		private IProxyGenerator _generator;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProxyHttpHandler" /> class.
		/// </summary>
		/// <param name="generator">The generator.</param>
		[ImportingConstructor]
		public ProxyHttpHandler(IProxyGenerator generator)
		{
			if (generator == null) throw new ArgumentNullException("generator");

			_generator = generator;
		}

		/// <summary>
		/// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler" /> instance.
		/// </summary>
		/// <returns>true if the <see cref="T:System.Web.IHttpHandler" /> instance is reusable; otherwise, false.</returns>
		public bool IsReusable
		{
			get { return true; }
		}

		/// <summary>
		/// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler" /> interface.
		/// </summary>
		/// <param name="context">An <see cref="T:System.Web.HttpContext" /> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
		public void ProcessRequest(HttpContext context)
		{
			if (context == null) throw new ArgumentNullException("context");

			context.Response.ContentType = "application/x-javascript";
			context.Response.Write(GetProxyJs(context));
		}

		private string GetProxyJs(HttpContext context)
		{
			if (_proxyJs == null)
				lock (_syncRoot)
					if (_proxyJs == null)
					{
						var script = _generator.GenerateProxyScript();

						if (!context.IsDebuggingEnabled)
						{
							var minifier = new Microsoft.Ajax.Utilities.Minifier();
							script       = minifier.MinifyJavaScript(script);
						}

						_proxyJs = script;
					}

			return _proxyJs;
		}
	}
}
