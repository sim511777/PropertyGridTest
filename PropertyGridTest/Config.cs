using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAT = System.ComponentModel.CategoryAttribute;
using DSP = System.ComponentModel.DisplayNameAttribute;
using DSC = System.ComponentModel.DescriptionAttribute;
using BRW = System.ComponentModel.BrowsableAttribute;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

// todo:
// ok - BrowsableAttribute : 해당 속성을 감출 수 있다.
// ok - CategoryAttribute : 카테고리 구분
// ok - DisplayNameAttribute : 프로퍼티 이름
// ok - DescriptionAttribute : 프로퍼티 설명
// ok - nullable 타입은 생성자에서 null 아닌 값으로 초기화 할 것
// ok - 사용자 정의 클래스 확장 속성 : [TypeConverter(typeof(ExpandableObjectConverter))]
// ok - string 변환을 바꾸려면 : TypeConverter 를 상속받아 새로운 컨버터 클래스 생성
// ok - 다른 에디터를 사용 하려면 : UITypeEditor 를 상속받아 새로운 에디터 클래스 작성
// ok - Enum 타입 표시 문자 바꾸기
// ok - 문자열 리스트 선택기
// ok - Enum 문자열 리스트 선택기
// ok - int 문자열 리스트 선택기
// - bool 타입 체크박스로
// - Flags Enum 체크박스로 바꿈


namespace PropertyGridTest {
    class Config {
        [BRW(false)] public int hidden { get; set; }

        const string cat1 = "1. Basic";
        [CAT(cat1)] [DSP("bool")] [DSC("Input bool value")] public bool boolVal { get; set; }
        [CAT(cat1)] [DSP("int")] [DSC("Input int value")] public int intVal { get; set; }
        [CAT(cat1)] [DSP("float")] [DSC("Input float value")] public float floatVal { get; set; }
        [CAT(cat1)] [DSP("string")] [DSC("Input string value")] public string stringVal { get; set; } = string.Empty;
        [CAT(cat1)] [DSP("Color")] [DSC("Input Color value")] public Color ColorVal { get; set; }
        [CAT(cat1)] [DSP("DateTime")] [DSC("Input DateTime value")] public DateTime DateTimeVal { get; set; }
        [CAT(cat1)] [DSP("Point")] [DSC("Input Point value")] public Point PointVal { get; set; }
        [CAT(cat1)] [DSP("Font")] [DSC("Input Font value")] public Font FontVal { get; set; } = SystemFonts.DefaultFont;

        const string cat2 = "2. Enum";
        [CAT(cat2)] [DSP("FormBorderStyle")] [DSC("Select FormBorderStyle value")] public FormBorderStyle fbstyle { get; set; }
        [CAT(cat2)] [DSP("Weekday")] [DSC("Weekday Custom Enum")] public Weekday weekday { get; set; }
        [CAT(cat2)] [DSP("Direction")] [DSC("Direction Custom Enum")] public Direction dir { get; set; }

        const string cat3 = "3. Array";
        [CAT(cat3)] [DSP("Integer Array")] [DSC("Input int array")] public int[] intArrayVal { get; set; } = new int[0];
        [CAT(cat3)] [DSP("String Array")] [DSC("Input string array")] public string[] strArrayVal { get; set; } = new string[0];

        const string cat4 = "4. Custom Class";
        [CAT(cat4)] [DSP("Person")] [DSC("Custom class")] public Person Personval { get; set; } = new Person();
        [CAT(cat4)] [DSP("People")] [DSC("Custom class array")] public Person[] PersonArrayVal { get; set; } = new Person[0];

        const string cat5 = "5. Custom Type Converter";
        [TypeConverter(typeof(PointConverter))]
        [CAT(cat5)] [DSP("Point")] [DSC("Point x,y displayed by '/")] public Point point2 { get; set; }
        
        [TypeConverter(typeof(YesNoConverter))]
        [CAT(cat5)] [DSP("bool")] [DSC("True/False -> Yes/No")] public bool boolVal2 { get; set; }
        
        [TypeConverter(typeof(EnumDisplayNameConverter))]
        [CAT(cat5)] [DSP("Direction")] [DSC("Enum Text Converter")] public Direction dirVal { get; set; }
        
        [TypeConverter(typeof(StringSelectConverter))]
        [StringList("Please", "Select", "Just", "One")]
        [CAT(cat5)] [DSP("string")] [DSC("You can select 1 string")] public string selText { get; set; } = "choose one";
        
        [TypeConverter(typeof(Int32SelectConverter))]
        [StringList("Please", "Select", "Just", "One")]
        [CAT(cat5)] [DSP("int")] [DSC("You can select 1 Int32")] public int selInt { get; set; } = 0;
        
        [TypeConverter(typeof(EnumSelectConverter))]
        [StringList("없음|고정싱글|고정3D|고정대화상자|리사이즈|고정도구창", "|")]
        [CAT(cat5)] [DSP("FormBorderStyle")] [DSC("Select FormBorderStyle value")] public FormBorderStyle fbstyle2 { get; set; }

        const string cat6 = "6. UITypeEditor";
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        [CAT(cat6)] [DSP("File")] [DSC("Select File")] public string fileName { get; set; } = string.Empty;
        
        [Editor(typeof(TextInputEditor), typeof(UITypeEditor))]
        [CAT(cat6)] [DSP("String")] [DSC("Input Text")] public string stringVal2 { get; set; } = string.Empty;
    }

    enum Weekday {
        Sunday,
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
    }

    enum Direction {
        [DSC("동쪽")] East,
        [DSC("서쪽")] West,
        [DSC("남쪽")] South,
        [DSC("북극 쪽")] North,
    }

    // TypeConverter
    [TypeConverter(typeof(ExpandableObjectConverter))]
    class Person {
        [DSP("이름")] public string name { get; set; } = string.Empty;
        [DSP("나이")] public int age { get; set; }
        [DSP("성별남자")] public bool sex { get; set; }
        public override string ToString() {
            return $"{name}, {age}, {(sex ? "Male" : "Female")}";
        }
    }
}
