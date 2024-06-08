# e-bx Technical Task

## Story:
As an unauthorised user requesting GET `/api/v1/{owner}/{repo}/contributors`
I want to see the authors of the last 100 commits in a GitHub repo of my choosing
So that I can determine who has recently contributed to the GitHub repo
Acceptance criteria:
Given a GitHub owner and repo
When requesting GET `/api/v1/{owner}/{repo}/contributors`
Then return the authors of the last 100 commits in the given repo
 
Given a GitHub owner or repo that does not exist in Github
When requesting GET /api/v1/{owner}/{repo}/contributors
Then return HTTP 404 Not Found

## Further improvements

I allocated about three hours to complete the assignment (excluding drafting this README), and therefore I have consciously made a few shortcuts.

Improvements that are worth considering are:
- Implementing unit tests
   - I opted to showcase integration tests instead of unit tests, as I believe that they bring more value to a real project. Said that, I still believe that unit tests bring a lot value, so this is definitely something I would add.
- Making use of SpecFlow.
- Implementing proper monitoring
   - I no longer use Ilogger in real projects nowadays, and always opt for the telemetry client or open telemetry instead.
- Implementing clean architecture
	- At the minute all the layers are stored in the WebApi project
- Implementing a proper retry mechanism for rate limiting
	- The API currently just returns 429 the gitbug client throws RateLimitExceededException 
- Moving the GitHub ApiOptions config in AppSettings.json and inject it, instead of hardcoding the pagination values
- The github token should come from a keyvault / pipeline library
- Better OpenApi doc
   - I only added the bare minimum for swagger

### Also..
I never used octokit before and I'm still wondering if that is a better option that using the github REST api directly. This feels like one of the "it depends" developer moment: the library seems to be well supported and documented, and it's surely making the code less cluttered. On the other side, it's yet-another-dependency, and the httpclient middleware already comes with Polly out of the box for retries.
