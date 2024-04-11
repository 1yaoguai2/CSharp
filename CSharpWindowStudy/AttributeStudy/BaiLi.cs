using System.Windows.Forms;

namespace AttributeStudy
{
    [AttributeClass.Hero]
    public class BaiLi
    {
        [AttributeClass.Skill]
        public void ShiYe()
        {
            MessageBox.Show("点亮一处视野", "技能描述");
        }

        [AttributeClass.Skill]
        public void JuJi()
        {
            MessageBox.Show("远距离狙击敌人", "技能描述");
        }
    }
}