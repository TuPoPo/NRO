
public class SkillOption
{
    public int param;

    public SkillOptionTemplate optionTemplate;

    public string optionString;

    public string getOptionString()
    {
        optionString ??= NinjaUtil.replace(optionTemplate.name, "#", string.Empty + param);
        return optionString;
    }
}
