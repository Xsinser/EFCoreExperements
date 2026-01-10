using EFCoreExperements.Core.Context;
using Microsoft.EntityFrameworkCore;

var optionsBuilder = new DbContextOptionsBuilder<MainContext>();
optionsBuilder.UseNpgsql("");

var context = new MainContext(optionsBuilder.Options);