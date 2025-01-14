using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdSubjects.DependencyInjection;
using IdSubjects.DirectoryLogon;
using IdSubjects.RealName;
using Microsoft.Extensions.DependencyInjection;

namespace AlphaIdPlatform.DependencyInjection;
public class AlphaIdPlatformBuilder(IServiceCollection services, 
    IdSubjectsBuilder idSubjects, 
    DirectoryLogonBuilder directoryLogon,
    RealNameBuilder realNameBuilder)
{
    public IServiceCollection Services { get; } = services;

    public IdSubjectsBuilder IdSubjects { get; } = idSubjects;

    public DirectoryLogonBuilder DirectoryLogon { get; } = directoryLogon;

    public RealNameBuilder RealName { get; } = realNameBuilder;
}
