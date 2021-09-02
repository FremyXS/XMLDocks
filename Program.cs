using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XMLDocks
{
    public enum Roles
    {
        developer, manager
    }
    public enum Elements
    {
        member, project, name
    }
    public 
    class Program
    {
        static void Main(string[] args)
        {
            Project.GetProjects(File.ReadAllLines(@"../../../projects.xml"));
            Member.SetMembers(@"../../../members.xml");
        }
    }
    public class Project
    {
        public string Name { get; }
        public List<OneMember> Members { get; private set; } = new List<OneMember>();
        public Project(string name)
        {
            Name = name;
        }
        public static List<Project> Projects { get; private set; } = new List<Project>();
        public static void GetProjects(string[] data)
        {
            foreach(var i in data)
            {
                var str = new List<string>();
                str.AddRange(i.Split(new char[] { '<', '>', '/', '=', '\"', '\t', ' '}, StringSplitOptions.RemoveEmptyEntries));

                if (str.Contains(Elements.project.ToString()) && str.Contains(Elements.name.ToString()))
                {
                    Projects.Add(new Project(str[2]));
                }
                else if (str.Contains(Elements.member.ToString()))
                {
                    Projects[Projects.Count - 1].Members.Add(new OneMember(str[2], str[4]));
                }
            }
        }

    }
    public class OneMember
    {
        public string Name { get; }
        public Roles Role { get; }
        public OneMember(string role, string name)
        {
            Name = name;
            Role = GetRole(role);
        }
        private Roles GetRole(string strRole)
        {
            if (strRole == Roles.developer.ToString())
                return Roles.developer;
            else if (strRole == Roles.manager.ToString())
                return Roles.manager;
            else
                throw new Exception();
        }

    }
    public class Member
    {
        public string Name { get; }
        public List<Role> Roles { get; private set; } = new List<Role>();
        public Member(string name)
        {
            Name = name;
        }
        public static List<Member> Members { get; private set; } = new List<Member>();
        public static void SetMembers(string dataLink)
        {
            foreach(var project in Project.Projects)
            {
                SetMember(project);
            }
            DataRecord(dataLink);
        }
        private static void SetMember(Project project)
        {
            Member newMember;
            foreach(var member in project.Members)
            {
                if(Members.Any(e => e.Name == member.Name))
                {
                    newMember = Members.Single(e => e.Name == member.Name);
                    Members.Remove(newMember);
                    newMember.Roles.Add(new Role(member.Role, project.Name));
                    Members.Add(newMember);
                }
                else
                {
                    newMember = new Member(member.Name);
                    newMember.Roles.Add(new Role(member.Role, project.Name));
                    Members.Add(newMember);
                }
                
            }
        }
        private static void DataRecord(string dataLink)
        {
            var listData = new List<string>();

            listData.Add("<projects>");

            foreach(var member in Members)
            {
                listData.Add(new string(' ', 4) + $"<member name=\"{member.Name}\">");
                DateRoles(listData, member);
                listData.Add(new string(' ', 4) + "</member>");
            }

            listData.Add("</projects>");

            File.WriteAllLines(dataLink, listData.ToArray());
        }
        private static void DateRoles(List<string> listData, Member member)
        {
            foreach(var role in member.Roles)
            {
                listData.Add(new string(' ', 8) + $"<role name=\"{role.Name}\" project=\"{role.ProjectName}\"/>");
            }
        }
    }
    public class Role
    {
        public Roles Name { get; }
        public string ProjectName { get; }
        public Role(Roles name, string projectName)
        {
            Name = name;
            ProjectName = projectName;
        }
    }
}
