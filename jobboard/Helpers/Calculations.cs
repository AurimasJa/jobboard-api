using jobboard.Auth;
using jobboard.Data.Models;

namespace jobboard.Helpers
{
    public interface ICalculations
    {
        List<AverageSalary> GetCityAverageSalaries(IReadOnlyList<Job> jobs);
        List<BiggestCompaniesDto> GetBiggestCompanies(IReadOnlyList<Job> jobs, IList<JobBoardUser> companies);
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
        public List<BiggestCompaniesDto> GetBiggestCompanies(IReadOnlyList<Job> jobs, IList<JobBoardUser> companies)
        {
            var biggestCompanies = jobs.Where(j => !j.IsHidden)
                                       .GroupBy(j => j.CompanyId)
                                       .OrderByDescending(g => g.Count())
                                       .Take(10)
                                       .Select(g => companies.Single(c => c.Id == g.Key))
                                       .ToList();

            var result = new List<BiggestCompaniesDto>();
            foreach (var company in biggestCompanies)
            {
                var total = jobs.Count(j => j.CompanyId == company.Id);
                var companyDto = new BiggestCompaniesDto(company.Id, company.CompanyName, company.Address, total);
                result.Add(companyDto);
            }

            return result;

        }
    }


}
