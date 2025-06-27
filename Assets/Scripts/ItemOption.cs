
public class ItemOption
{
    public int param;

    public sbyte active;

    public sbyte activeCard;

    public ItemOptionTemplate optionTemplate;

    public ItemOption()
    {
    }
    public bool IsValidOption()
    {
        return this != null && optionTemplate != null && optionTemplate.id != 21 && optionTemplate.id != 200 && optionTemplate.id != 72 && optionTemplate.id != 57 && optionTemplate.id != 58 && optionTemplate.id != 34 && optionTemplate.id != 35 && optionTemplate.id != 36 && optionTemplate.id != 102 && optionTemplate.id != 107;
    }

    public ItemOption(int optionTemplateId, int param)
    {
        if (optionTemplateId == 22)
        {
            optionTemplateId = 6;
            param *= 1000;
        }
        if (optionTemplateId == 23)
        {
            optionTemplateId = 7;
            param *= 1000;
        }
        this.param = param;
        optionTemplate = GameScr.gI().iOptionTemplates[optionTemplateId];
    }

    public string getOptionString()
    {
        return NinjaUtil.replace(optionTemplate.name, "#", param + string.Empty);
    }

    public string getOptionName()
    {
        return NinjaUtil.replace(optionTemplate.name, "+#", string.Empty);
    }

    public string getOptiongColor()
    {
        return NinjaUtil.replace(optionTemplate.name, "$", string.Empty);
    }
}
