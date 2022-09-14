using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ddPoliglotV6.BL.Enums;
using ddPoliglotV6.BL.Helpers;
using ddPoliglotV6.BL.Managers;
using ddPoliglotV6.BL.Models;
using ddPoliglotV6.Data;
using ddPoliglotV6.Data.Models;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace ddPoliglotV6.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ddPoliglotDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> UserManager;

        public List<tmp1> WrdOrdered = new List<tmp1>();
        public List<tmp1> WrdNotListed = new List<tmp1>();
        public List<Word> WrdSkiped = new List<Word>();
        public List<string> WrdTypes;
        public Dictionary<string, List<tmp1>> WrdDict = new Dictionary<string, List<tmp1>>();

        public IndexModel(
            ddPoliglotDbContext context,
            ILogger<IndexModel> logger,
            IHostingEnvironment hostingEnvironment,
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager
            )
        {
            _logger = logger;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            UserManager = userManager;
            this.ImageBaseUrl = _configuration["lessonsImageUrl"];
        }

        public List<Lesson> SkLessons { get; set; }
        public List<Lesson> EnLessons { get; set; }
        public string ImageBaseUrl { get; set; }

        public async Task OnGetAsync()
        {

            var pageLanguage = Enum.Parse(typeof(Languages), HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture.Name);
            ViewData["sk-lessons-folder-alias"] = Infrastructure.Route.RoutersTree.GetLessonsFolderAliasName(pageLanguage.ToString(), Languages.sk.ToString());

            var querySk = from l in _context.Lessons
                        where l.NativeLanguageID == (int)pageLanguage
                        && l.LanguageID == (int)Languages.sk
                        && l.ParentID == 0
                        select l;

            this.SkLessons = await querySk.AsNoTracking().Take(6).ToListAsync();

            var queryEn = from l in _context.Lessons
                        where l.NativeLanguageID == (int)pageLanguage
                        && l.LanguageID == (int)Languages.en
                        && l.ParentID == 0
                        select l;

            this.EnLessons = await queryEn.AsNoTracking().Take(6).ToListAsync();






           // TestGetRepetition();

            //var spaSettings = new SpaAppSetting(
            //    new Language() { Code = "ru", LanguageID = 2 },
            //    new Language() { Code = "sk", LanguageID = 4 }
            //    );

            //var userId = new Guid((await this.UserManager.FindByNameAsync(User?.Identity?.Name)).Id);
            //var list = await (new WordManager(_context, _configuration, _hostingEnvironment))
            //    .GetListForRepetition(spaSettings, userId, 10);

            // await (new WordManager(_context, _configuration, _hostingEnvironment))
            // .AddListFromTextWithPhrases(wwphText, spaSettings, true);
        }

        #region test calc repetition list
        
        public void TestGetRepetition()
        {
            var data = InitTestGetRepetitionData();
            var lessons = new List<testLesson>();
            var lessonQty = 15;
            for (int lessonNum = 1; lessonNum < lessonQty; lessonNum++)
            {
                var lessonData = GetLessonRepetition(data);
                var lessonItem = new testLesson { Words = new List<testWord>(), Num = lessonNum };
                foreach (var item in lessonData)
                {
                    // save repetition
                    item.LastRepeat = lessonNum;
                    lessonItem.Words.Add(item);
                }

                lessons.Add(lessonItem);
            }


            foreach (var lesson in lessons)
            {
                Debug.Print($"+++++++++++++ LESSON {lesson.Num}");
                Debug.Print("");

                foreach (var word in lesson.Words)
                {
                    Debug.Print($"     item {word.Rate} {word.Text} Grade:{word.Grade}");
                }

                Debug.Print("-------------------------------------------------------");
                Debug.Print("");
            }
        }

        private List<testWord> GetLessonRepetition(List<testWord> data)
        {
            var result = data.OrderBy(x=>x.LastRepeat).ThenBy(x => x.Grade).ThenBy(x=>x.Rate).Take(2).ToList();
            
            return result;
        }

        private List<testWord> InitTestGetRepetitionData()
        {
            var result = new List<testWord>();
            for (int i = 1; i < 21; i++)
            {
                result.Add(new testWord
                {
                    Rate = i,
                    Text = $"word_{i}",
                    Grade = i
                });
            }

            return result;
        }

        public class testWord
        { 
            public string Text { get; set; }
            public int Rate { get; set; }
            public int Grade { get; set; }
            public int LastRepeat { get; set; }
        }

        public class testLesson
        {
            public int Num { get; set; }
            public List<testWord> Words { get; set; }
        }

        #endregion test calc repetition list

        #region Add Base words with phrases
        public string wwphText = @"
*10
 +Amerika 
    -Tom je Američan. Je z Ameriky.
    -Bill a Ela sú Američania.
    -Amerika je veľký štát.
+Auto
    -Mal som americké auto.
    -Moje auto má 10 rokov.
    -Som v aute.
+Belgicko
    -Belgicko je v Európe.
    -Môj kamarát je z Belgicka.
    -Jeho kamarátka je Belgičanka.
+Bicykel
    -Náš manažér má dobrý bicykel.
    -Náš učiteľ používa bicykel.
-Idem na bicykli. 
         
            ";
          #endregion Add Base words with phrases

        #region Base words

        private void ShowBaseWordsFunction()
        {
            // var words = new List<(string text, int rate)>();
            var wordsToProcess = BaseWords.Select(x =>
            {
                var arr = x.Split("::");
                return new tmp1()
                {
                    Text = arr[0],
                    Type = arr[1],
                    Rate = int.MaxValue
                };
            }).ToList();

            var words = _context.Words.Select(x => x).OrderBy(x=>x.Rate).ToList();
            wordsToProcess.ForEach(item => 
            {
                var wrd = words.FirstOrDefault(x =>x.Text.ToLower() == item.Text.ToLower());
                if (wrd != null)
                {
                    item.Rate = wrd.Rate;
                    wrd.HashCodeSpeed1 = -10;
                }
            });
            this.WrdTypes = wordsToProcess.GroupBy(t => t.Type).Select(grp => grp.First().Type).ToList();
            this.WrdTypes.ForEach(type => 
            { 
                var wrd = wordsToProcess.Where(x => x.Type == type).OrderBy(x => x.Rate).ToList();
                this.WrdDict.Add(type, wrd);
            });

            this.WrdOrdered = wordsToProcess.Where(x => x.Rate < int.MaxValue).OrderBy(x => x.Rate).ToList();
            this.WrdNotListed = wordsToProcess.Where(x => x.Rate == int.MaxValue).OrderBy(x => x.Rate).ToList();
            this.WrdSkiped = words.Where(x=>x.HashCodeSpeed1 != -10 && x.Rate < 500).ToList();
        }

        public class tmp1
        {
            public string Text { get; set; }
            public int Rate { get; set; }
            public string Type { get; set; }
        }

        private string[] BaseWords = {
"about::prep",
"add::v",
"afternoon::n",
"again::adv",
"alien::n",
"alphabet::n",
"an::det",
"and::conj",
"angry::adj",
"animal::n",
"answer::n",
"apartment::n",
"apple::n",
"arm::n",
"armchair::n",
"ask::v",
"baby::n",
"badminton::n",
"bag::n",
"ball::n",
"balloon::n",
"banana::n",
"baseball::n",
"basketball::n",
"bat::n",
"bath::n",
"bathroom::n",
"be::v",
"beach::n",
"bean::n",
"bear::n",
"beautiful::adj",
"bed::n",
"bedroom::n",
"bee::n",
"behind::prep",
"between::prep",
"big::adj",
"bike::n",
"bird::n",
"birthday::n",
"black::adj",
"blue::adj",
"board::n",
"boat::n",
"body::n",
"book::n",
"bookcase::n",
"bookshop::n",
"boots::n",
"bounce::v",
"box::n",
"boy::n",
"bread::n",
"breakfast::n",
"brother::n",
"brown::adj",
"burger::n",
"bus::n",
"but::conj",
"bye::excl",
"cake::n",
"camera::n",
"can::v",
"candy::n",
"car::n",
"carrot::n",
"cat::n",
"catch::v",
"chair::n",
"chicken::n",
"child::n",
"children::n",
"chips::n",
"chocolate::n",
"choose::v",
"clap::v",
"class::n",
"classmate::n",
"classroom::n",
"clean::adj",
"clock::n",
"close::v",
"closed::adj",
"clothes::n",
"coconut::n",
"colour::n",
"come::v",
"complete::v",
"computer::n",
"cool::adj",
"correct::adj",
"count::v",
"cousin::n",
"cow::n",
"crayon::n",
"crocodile::n",
"cross::n",
"cupboard::n",
"dad::n",
"day::n",
"desk::n",
"dinner::n",
"dirty::adj",
"do::v",
"dog::n",
"doll::n",
"donkey::n",
"door::n",
"double::adj",
"draw::v",
"drawing::n",
"dress::n",
"drink::n",
"drive::v",
"duck::n",
"ear::n",
"eat::v",
"egg::n",
"elephant::n",
"end::n",
"English::adj",
"enjoy::v",
"eraser::n",
"evening::n",
"example::n",
"eye::n",
"face::n",
"family::n",
"fantastic::adj",
"father::n",
"favourite::adj",
"find::v",
"fish::n",
"fishing::n",
"flat::n",
"floor::n",
"flower::n",
"fly::v",
"food::n",
"foot::n",
"feet::n",
"football::n",
"for::prep",
"friend::n",
"fries::n",
"frog::n",
"from::prep",
"fruit::n",
"fun::adj",
"funny::adj",
"game::n",
"garden::n",
"get::v",
"giraffe::n",
"girl::n",
"give::v",
"glasses::n",
"go::v",
"goat::n",
"good::adj",
"goodbye::excl",
"grandfather::n",
"grandma::n",
"grandmother::n",
"grandpa::n",
"grape::n",
"gray::adj",
"great::adj",
"green::adj",
"grey::adj",
"guitar::n",
"hair::n",
"hall::n",
"hand::n",
"handbag::n",
"happy::adj",
"hat::n",
"have::v",
"he::pron",
"head::n",
"helicopter::n",
"hello::excl",
"her::poss",
"here::adv",
"hers::pron",
"hi::excl",
"him::pron",
"hippo::n",
"his::poss",
"hit::v",
"hobby::n",
"hockey::n",
"hold::v",
"home::n",
"hooray::excl",
"horse::n",
"house::n",
"how::int",
"how many::int",
"how old::int",
"I::pron",
"ice cream::n",
"in::prep",
"it::pron",
"its::poss",
"jacket::n",
"jeans::n",
"jellyfish::n",
"juice::n",
"jump::v",
"keyboard::n",
"kick::v",
"kid::n",
"kitchen::n",
"kite::n",
"kiwi::n",
"know::v",
"lamp::n",
"learn::v",
"leg::n",
"lemon::n",
"lemonade::n",
"lesson::n",
"let::v",
"letter::n",
"like::prep",
"lime::n",
"line::n",
"listen::v",
"live::v",
"living room::n",
"lizard::n",
"long::adj",
"look::v",
"lorry::n",
"a lot::adv",
"a lot of::det",
"lots::adv",
"lots of::det",
"love::v",
"lunch::n",
"make::v",
"man::n",
"men::n",
"mango::n",
"many::det",
"mat::n",
"me::pron",
"me too::dis",
"meat::n",
"meatballs::n",
"milk::n",
"mine::pron",
"mirror::n",
"Miss::title",
"monkey::n",
"monster::n",
"morning::n",
"mother::n",
"motorbike::n",
"mouse::n",
"mouth::n",
"Mr::title",
"Mrs::title",
"mum::n",
"music::n",
"my::poss",
"name::n",
"new::adj",
"next to::prep",
"nice::adj",
"night::n",
"no::adv",
"nose::n",
"not::adv",
"now::adv",
"number::n",
"of::prep",
"oh::dis",
"OK::adj",
"old::adj",
"on::prep",
"one::det",
"onion::n",
"open::adj",
"or::conj",
"orange::adj",
"our::poss",
"ours::pron",
"page::n",
"paint::n",
"painting::n",
"paper::adj",
"pardon::int",
"park::n",
"part::n",
"Pat::n",
"pea::n",
"pear::n",
"pen::n",
"pencil::n",
"person::n",
"people::n",
"pet::n",
"phone::n",
"photo::n",
"piano::n",
"pick up::v",
"picture::n",
"pie::n",
"pineapple::n",
"pink::adj",
"plane::n",
"play::v",
"playground::n",
"please::dis",
"point::v",
"polar bear::n",
"poster::n",
"potato::n",
"purple::adj",
"put::v",
"question::n",
"radio::n",
"read::v",
"really::adv",
"red::adj",
"rice::n",
"ride::v",
"right::dis",
"robot::n",
"room::n",
"rubber::n",
"rug::n",
"ruler::n",
"run::v",
"sad::adj",
"sand::n",
"sausage::n",
"say::v",
"scary::adj",
"school::n",
"sea::n",
"see::v",
"see you::excl",
"sentence::n",
"she::pron",
"sheep::n",
"shell::n",
"ship::n",
"shirt::n",
"shoe::n",
"shop::n",
"short::adj",
"shorts::n",
"show::v",
"silly::adj",
"sing::v",
"sister::n",
"sit::v",
"skateboard::n",
"skateboarding::n",
"skirt::n",
"sleep::v",
"small::adj",
"smile::n",
"snake::n",
"so::dis",
"soccer::n",
"sock::n",
"sofa::n",
"some::det",
"song::n",
"sorry::adj",
"spell::v",
"spider::n",
"sport::n",
"stand::v",
"start::v",
"stop::v",
"store::n",
"story::n",
"street::n",
"sun::n",
"sweet::n",
"swim::v",
"table::n",
"tablet::n",
"tail::n",
"take a photo/picture::v",
"talk::v",
"teacher::n",
"teddy (bear)::n",
"television/TV::n",
"tell::v",
"tennis::n",
"tennis racket::n",
"thank you::dis",
"thanks::dis",
"that::det",
"the::det",
"their::poss",
"theirs::pron",
"them::pron",
"then::dis",
"there::adv",
"these::det",
"they::pron",
"thing::n",
"this::det",
"those::det",
"throw::v",
"tick::n",
"tiger::n",
"to::prep",
"today::adv",
"tomato::n",
"too::adv",
"toy::n",
"train::n",
"tree::n",
"trousers::n",
"truck::n",
"try::n",
"T-shirt::n",
"ugly::adj",
"under::prep",
"understand::v",
"us::pron",
"very::adv",
"walk::v",
"wall::n",
"want::v",
"watch::n",
"water::n",
"watermelon::n",
"wave::v",
"we::pron",
"wear::v",
"well::dis",
"well done::dis",
"what::int",
"where::int",
"which::int",
"white::adj",
"who::int",
"whose::int",
"window::n",
"with::prep",
"woman/women::n",
"word::n",
"would like::v",
"wow::excl",
"write::v",
"year::n",
"yellow::adj",
"yes::adv",
"you::pron",
"young::adj",
"your::poss",
"yours::pron",
"zebra::n",
"zoo::n",
        };

        #endregion Base words

        #region Functions
 
        #endregion Functions
    }
}



