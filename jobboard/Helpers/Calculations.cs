using jobboard.Auth;
using jobboard.Data.Models;

namespace jobboard.Helpers
{
    public interface ICalculations
    {
        List<AverageSalary> GetCityAverageSalaries(IReadOnlyList<Job> jobs);
        List<CompanyDto> GetBiggestCompanies(IReadOnlyList<Job> jobs, IList<JobBoardUser> companies);
    }

    public class Calculations : ICalculations
    {
        public Calculations()
        {
        }

        public List<AverageSalary> GetCityAverageSalaries(IReadOnlyList<Job> jobs)
        {
            var avg = jobs.GroupBy(j => j.City)
                                   .Select(x => new AverageSalary { CityName = x.Key, AverageCitySalary = x.Average(j => j.Salary) })
                                       .Take(10)
                                   .ToList();
            return avg;
        }
        public List<CompanyDto> GetBiggestCompanies(IReadOnlyList<Job> jobs, IList<JobBoardUser> companies)
        {
            var biggestCompanies = jobs.GroupBy(j => j.CompanyId)
                                       .OrderByDescending(g => g.Count())
                                       .Take(10)
                                       .Select(g => companies.Single(c => c.Id == g.Key))
                                       .ToList();

            var result = new List<CompanyDto>();
            foreach (var company in biggestCompanies)
            {
                var companyDto = new CompanyDto(company.Id, company.CompanyName, company.Address, company.Email);
                result.Add(companyDto);
            }

            return result;

        }
    }


}
