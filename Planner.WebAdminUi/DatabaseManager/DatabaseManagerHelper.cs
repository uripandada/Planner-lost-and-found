using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Planner.Common.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planner.WebAdminUi.DatabaseManager
{
	/// <summary>
	/// WARNING 
	/// WARNING 
	/// WARNING 
	/// Before you start judging why are there database shenanigans here -
	/// Since the hotel group tenant doesn't exist yet and database context must be manually created, you can't just simply setup injection
	/// to give you proper database context in the Application project. DatabaseContext can't be referenced from the Application and this can't be achieved in a simple way.
	/// WARNING 
	/// WARNING 
	/// WARNING 
	/// </summary>

}
