using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdSubjects.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace AlphaIdPlatform.DependencyInjection;
public class AlphaIdPlatformBuilder(IServiceCollection services, IdSubjectsBuilder idSubjects)
{
    public IServiceCollection Services { get; } = services;

    public IdSubjectsBuilder IdSubjects { get; } = idSubjects;
}
