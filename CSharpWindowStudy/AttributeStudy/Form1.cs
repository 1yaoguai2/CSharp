using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace AttributeStudy
{
    public partial class Form1 : Form
    {
        private List<Type> heroTypes = new List<Type>();
        private object selectedHero;

        public Form1()
        {
            InitializeComponent();

            //通过反射属性加载所有英雄
            /*
             * Assembly 程序集
             * GetExecutingAssembly() ，执行当前代码的程序集
             * GetTypes() ，所有的类型
             * Where 扩展方法结合Lamdba表达式进行筛选
             */
            heroTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetCustomAttributes(typeof(AttributeClass.HeroAttribute), false).Any()).ToList();

            //初始化英雄列表
            heroListBox.Items.AddRange(heroTypes.Select(t => t.Name).ToArray());
        }


        private void heroListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (heroListBox.SelectedIndex == -1) return;     //如果未选定则跳出
            //获取当前旋转的英雄
            var selectedHeroType = heroTypes[heroListBox.SelectedIndex];
            selectedHero = Activator.CreateInstance(selectedHeroType);
            
            //获取改英雄类型的所有技能方法
            var skillMethods = selectedHeroType.GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(AttributeClass.SkillAttribute), false).Any()).ToList();
            
            //初始划技能方法
            skillListBox.Items.Clear();
            skillListBox.Items.AddRange(skillMethods.Select(m => m.Name).ToArray());
            
        }

        private void skillListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(skillListBox.SelectedIndex == -1) return;
            
            //获取当前点击的技能
            var selectedSikllMethod = selectedHero.GetType().GetMethod(skillListBox.SelectedItem.ToString());
            
            //调用该技能方法
            selectedSikllMethod?.Invoke(selectedHero,null);

        }
    }
    /*
     * 特性，为程序元素额外添加声明信息的一种方式，类似生活中的标签。
     * 反射，是一种能力，运行时获取程序集中的元信息。
     * 程序运行时会产生一个应用程序域，里面有程序集Assembly，反射能读取程序集当中的元信息，特性也属于元信息。
     */
}