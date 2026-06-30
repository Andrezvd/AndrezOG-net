namespace AndrezOG.Domain.Model.Skills;

using NpgsqlTypes;

public enum SkillType
{
    [PgName("Technology")]
    Technology,

    [PgName("Methodology")]
    Methodology,

    [PgName("SoftSkill")]
    SoftSkill,

    [PgName("Certification")]
    Certification
}
