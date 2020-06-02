using System;
using System.Collections.Generic;
using System.Text;

namespace MPSInverterPoller
{
    public class MpsCrc
    {
        private static char[] crc_tb = { '\0', (char)'အ', (char)'⁂', (char)'っ', (char)'䂄',
    '傥', (char)'惆', (char)'烧',(char) 33032, (char) 37161,(char)  41290,(char)  45419,(char)  49548,
   (char)  53677,(char)  57806, (char) 61935, (char)'ሱ', (char)'Ȑ', (char)'㉳', (char)'≒', (char)'劵',
    '䊔', (char)'狷', (char)'拖',(char)  37689, (char) 33560, (char)45947, (char)41818, (char)54205,
    (char) 50076,(char)  62463, (char)(char) 58334, (char)'③', (char)'㑃', (char)'Р', (char)'ᐁ', (char)'擦',
    '瓇', (char)'䒤', (char)'咅', (char) 42346, (char) 46411, (char)34088, (char)38153, (char)58862,
    (char) 62927, (char) 50604, (char) 54669, (char)'㙓', (char)'♲', (char)'ᘑ', (char)'ذ', (char)'盗',
    '曶', (char)'嚕', (char)'䚴',(char)  46939, (char)42874, (char)38681, (char)34616, (char)63455,
    (char) 59390, (char) 55197, (char)51132, (char)'䣄', (char)'壥', (char)'梆', (char)'碧', (char)'ࡀ',
    'ᡡ', (char)'⠂', (char)'㠣', (char) 51660, (char)55789, (char)59790, (char)63919, (char)35144,
  (char)   39273, (char) 43274,(char) 47403, (char)'嫵', (char)'䫔', (char)'窷', (char)'檖', (char)'ᩱ',
    '੐', (char)'㨳', (char)'⨒',(char)  56317,(char) 52188, (char)64447, (char)60318, (char)39801,
 (char)    35672, (char) 47931,(char) 43802, (char)'沦', (char)'粇', (char)'䳤', (char)'峅', (char)'Ⱒ',
    '㰃', (char)'ౠ', (char)'᱁', (char) 60846, (char)64911, (char)52716, (char)56781, (char)44330,
  (char)   48395, (char) 36200, (char)40265, (char)'纗', (char)'溶', (char)'廕', (char)'仴', (char)'㸓',
    '⸲', (char)'ṑ', (char)'๰', (char) 65439,(char) 61374, (char)57309, (char)53244, (char)48923,
  (char)   44858, (char) 40793, (char)36728, (char)37256, (char)33193, (char)45514, (char)41451, (char)53516,
  (char)   49453, (char) 61774, (char)57711, (char)'ႀ', (char)'¡', (char)'ヂ', (char)'⃣', (char)'倄',
    '䀥', (char)'灆', (char)'恧', (char) 33721,(char) 37784, (char)41979, (char)46042, (char)49981,
  (char)   54044, (char) 58239,(char) 62302, (char)'ʱ', (char)'ነ', (char)'⋳', (char)'㋒', (char)'䈵',
    '刔', (char)'扷', (char)'牖',(char)  46570, (char)42443, (char)38312, (char)34185, (char)62830,
  (char)   58703, (char) 54572, (char)50445, (char)'㓢', (char)'Ⓝ', (char)'ᒠ', (char)'ҁ', (char)'瑦',
    '摇', (char)'吤', (char)'䐅', (char) 42971, (char)47098, (char)34713, (char)38840, (char)59231,
   (char)  63358, (char) 50973, (char)55100, (char)'⛓', (char)'㛲', (char)'ڑ', (char)'ᚰ', (char)'晗',
    '癶', (char)'䘕', (char)'嘴', (char) 55628, (char)51565, (char)63758, (char)59695, (char)39368,
  (char)   35305,(char)  47498, (char)43435, (char)'塄', (char)'䡥', (char)'砆', (char)'栧', (char)'ᣀ',
    '࣡', (char)'㢂', (char)'⢣', (char) 52093, (char)56156, (char)60223, (char)64286, (char)35833,
   (char)  39896,(char)  43963, (char)48026, (char)'䩵', (char)'婔', (char)'樷', (char)'稖', (char)'૱',
    '᫐', (char)'⪳', (char)'㪒', (char) 64814, (char)60687, (char)56684, (char)52557, (char)48554,
  (char)   44427, (char) 40424, (char)36297, (char)'簦', (char)'氇', (char)'層', (char)'䱅', (char)'㲢',
    'ⲃ', (char)'᳠', (char)'ು', (char) 61215, (char)65342, (char)53085, (char)57212, (char)44955,
  (char)   49082, (char) 36825, (char)40952, (char)'渗', (char)'縶', (char)'乕', (char)'年', (char)'⺓',
    '㺲', (char)'໑', (char)'Ự' };


        public static byte[] caluCRC(byte[] pByte)
        {
            try
            {
                int len = pByte.Length;




                int i = 0;
                int crc = 0;
                while (len-- != 0)
                {
                    int da = 0xFF & (0xFF & crc >> 8) >> 4;
                    crc <<= 4;
                    crc ^= crc_tb[(0xFF & (da ^ pByte[i] >> 4))];

                    da = 0xFF & (0xFF & crc >> 8) >> 4;
                    crc <<= 4;
                    int temp = 0xFF & (da ^ pByte[i] & 0xF);
                    crc ^= crc_tb[temp];
                    i++;
                }
                int bCRCLow = 0xFF & crc;
                int bCRCHign = 0xFF & crc >> 8;
                if ((bCRCLow == 40) || (bCRCLow == 13) || (bCRCLow == 10))
                {
                    bCRCLow++;
                }
                if ((bCRCHign == 40) || (bCRCHign == 13) || (bCRCHign == 10))
                {
                    bCRCHign++;
                }
                byte[] dat = new byte[] { (byte)bCRCHign, (byte)bCRCLow };
                return dat;
                //crc = (0xFF & bCRCHign) << 8;
                //return crc + bCRCLow;
            }
            catch (Exception ex)
            {

            }
            return new byte[2] { 0, 0 };
        }

    }
}
