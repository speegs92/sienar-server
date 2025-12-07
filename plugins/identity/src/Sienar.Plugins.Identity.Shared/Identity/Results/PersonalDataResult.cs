using Sienar.Data;
using Sienar.Infrastructure;

namespace Sienar.Identity.Results;

public class PersonalDataResult : IResult
{
	public DownloadFile? PersonalDataFile { get; set; }

	public PersonalDataResult(DownloadFile file)
	{
		PersonalDataFile = file;
	}
}