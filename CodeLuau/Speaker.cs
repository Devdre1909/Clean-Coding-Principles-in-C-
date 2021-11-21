using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeLuau
{
  public class Speaker
  {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public int? yearsOfExperience { get; set; }
    public bool HasBlog { get; set; }
    public string BlogURL { get; set; }
    public WebBrowser Browser { get; set; }
    public List<string> Certifications { get; set; }
    public string Employer { get; set; }
    public int RegistrationFee { get; set; }
    public List<Session> Sessions { get; set; }

    public RegisterResponse Register(IRepository repository)
    {

      RegisterError? errorWhenRegisting = ValidateRegistration();
      if (errorWhenRegisting != null) return new RegisterResponse(errorWhenRegisting);


      if (yearsOfExperience <= 1)
      {
        RegistrationFee = 500;
      }
      else if (yearsOfExperience >= 2 && yearsOfExperience <= 3)
      {
        RegistrationFee = 250;
      }
      else if (yearsOfExperience >= 4 && yearsOfExperience <= 5)
      {
        RegistrationFee = 100;
      }
      else if (yearsOfExperience >= 6 && yearsOfExperience <= 9)
      {
        RegistrationFee = 50;
      }
      else
      {
        RegistrationFee = 0;
      }

      int? speakerId = repository.SaveSpeaker(this);

      return new RegisterResponse((int)speakerId);
    }

    private RegisterError? ValidateRegistration()
    {

      RegisterError? error = ValidateData();
      if (error != null) return error;

      bool speakerAppersQualified = AppersExceptional() || HasObviousRedFlags();
      if (!speakerAppersQualified) return RegisterError.SpeakerDoesNotMeetStandards;

      bool atLeastOneSessionApproved = ApproveSessions();
      if (!atLeastOneSessionApproved) return RegisterError.NoSessionsApproved;

      return null;

    }

    private bool ApproveSessions()
    {
      foreach (var session in Sessions)
      {
        session.Approved = !SessionsIsAboutOldTechnology(session);
      }

      return Sessions.Any(s => s.Approved);
    }

    private bool SessionsIsAboutOldTechnology(Session session)
    {
      var oldTechnologies = new List<string>() { "Cobol", "Punch Cards", "Commodore", "VBScript" };
      foreach (var tech in oldTechnologies)
      {
        if (session.Title.Contains(tech) || session.Description.Contains(tech))
        {
          return true;
        }
      }
      return false;
    }

    private bool HasObviousRedFlags()
    {

      string emailDomain = Email.Split('@').Last();
      var oldDomains = new List<string>() { "aol.com", "prodigy.com", "compuserve.com" };

      if (oldDomains.Contains(emailDomain) && (Browser.Name == WebBrowser.BrowserName.InternetExplorer && Browser.MajorVersion < 9)) return true;

      return false;
    }

    private bool AppersExceptional()
    {
      if (yearsOfExperience > 10) return true;
      if (HasBlog) return true;
      if (Certifications.Count() > 3) return true;

      var preferredEmployers = new List<string>() { "Pluralsight", "Microsoft", "Google" };
      if (preferredEmployers.Contains(Employer)) return true;

      return false;
    }

    private RegisterError? ValidateData()
    {
      if (string.IsNullOrWhiteSpace(FirstName)) return RegisterError.FirstNameRequired;
      if (string.IsNullOrWhiteSpace(LastName)) return RegisterError.LastNameRequired;
      if (string.IsNullOrWhiteSpace(Email)) return RegisterError.EmailRequired;
      if (!Sessions.Any()) return RegisterError.NoSessionsProvided;
      return null;
    }

  }
}