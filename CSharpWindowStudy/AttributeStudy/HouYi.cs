using System.Windows.Forms;

namespace AttributeStudy
{
    [AttributeClass.Hero]
    public class HouYi
    {
        [AttributeClass.Skill]
        public void SheJian()
        {
            MessageBox.Show("发射箭矢", "技能描述");
        }

        [AttributeClass.Skill]
        public void SheRi()
        {
            MessageBox.Show("超远距离伤害", "技能描述");
        }
    }
}