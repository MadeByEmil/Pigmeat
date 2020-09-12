﻿// Copyright (C) 2020 Emil Sayahi
/*
This file is part of Pigmeat.

    Pigmeat is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Pigmeat is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Pigmeat.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.IO;
using System.Net;
using LibGit2Sharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pigmeat.Core;
using SharpScss;

namespace Pigmeat
{
    /// <summary>
    /// The <c>Program</c> class.
    /// Connects user inputs via command calls to the <c>Pigmeat.Core</c> library.
    /// The vanilla Pigmeat tool and standard implementation.
    /// </summary>
    static class Program
    {
        static DateTime LastFileWatcherEvent { get; set; }
        /// <summary>
        /// Handle primary tool information, such as command inputs and current directory
        /// </summary>

        static void Main(string[] args)
        {
            try
            {
                switch(args[0])
                {
                    case "help":
                    case "h":
                        break;
                    // For `install`, switch directories with three arguments (command, path, and something else)
                    case "install":
                    case "i":
                        switch(args.Length)
                        {
                            case var expression when (args.Length >= 3):
                                try
                                {
                                    Directory.SetCurrentDirectory(args[1]);
                                }
                                catch (DirectoryNotFoundException e)
                                {
                                    Console.WriteLine("The specified directory does not exist: " + args[1] + "\n" + e);
                                    Environment.Exit(128); // Invalid argument
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    // Other commands will switch directories with two arguments (command & path)
                    default:
                        switch(args.Length)
                        {
                            case 2:
                                try
                                {
                                    Directory.SetCurrentDirectory(args[1]);
                                }
                                catch (DirectoryNotFoundException e)
                                {
                                    Console.WriteLine("The specified directory does not exist: " + args[1] + "\n" + e);
                                    Environment.Exit(128); // Invalid argument
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                }
                GetCommand(args);
            }
            catch(NotImplementedException)
            {
                
            }
            catch(IndexOutOfRangeException)
            {
                GetCommand(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Environment.Exit(1); // General errors
            }
        }

        /// <summary>
        /// Perform actions specified by command calls
        /// <para> See <see cref="Main(string[])"/> </para>
        /// </summary>
        static void GetCommand(string[] args)
        {
            switch (args.Length)
            {
                case 0:
                    Console.WriteLine("Pigmeat  Copyright (C) 2020  Emil Sayahi\n    This program comes with ABSOLUTELY NO WARRANTY; for details type `pigmeat w'.\n    This is free software, and you are welcome to redistribute it\n    under certain conditions; type `pigmeat t' for details.\n");
                    Help(args);
                    break;
                default:
                    switch (args[0]) {
                        case "new":
                        case "n":
                            New();
                            break;
                        case "build":
                        case "b":
                            Build(true);
                            break;
                        case "serve":
                        case "s":
                            ServeWatch();
                            break;
                        case "install":
                        case "i":
                            Install(args);
                            break;
                        case "clean":
                        case "c":
                            Clean();
                            break;
                        case "help":
                        case "h":
                            Help(args);
                            break;
                        case "about":
                            About();
                            break;
                        case "warranty":
                        case "w":
                            Warranty();
                            break;
                        case "terms":
                        case "t":
                            Terms();
                            break;
                        default:
                            Help(args);
                            Environment.Exit(127); // Command not found
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// Output the terms and conditions of the GPL 3.0 license
        /// <para> See <see cref="GetCommand(string[])"/> </para>
        /// </summary>
        static void Terms()
        {
            Console.WriteLine("                       TERMS AND CONDITIONS\r\n\r\n  0. Definitions.\r\n\r\n  \"This License\" refers to version 3 of the GNU General Public License.\r\n\r\n  \"Copyright\" also means copyright-like laws that apply to other kinds of\r\nworks, such as semiconductor masks.\r\n\r\n  \"The Program\" refers to any copyrightable work licensed under this\r\nLicense.  Each licensee is addressed as \"you\".  \"Licensees\" and\r\n\"recipients\" may be individuals or organizations.\r\n\r\n  To \"modify\" a work means to copy from or adapt all or part of the work\r\nin a fashion requiring copyright permission, other than the making of an\r\nexact copy.  The resulting work is called a \"modified version\" of the\r\nearlier work or a work \"based on\" the earlier work.\r\n\r\n  A \"covered work\" means either the unmodified Program or a work based\r\non the Program.\r\n\r\n  To \"propagate\" a work means to do anything with it that, without\r\npermission, would make you directly or secondarily liable for\r\ninfringement under applicable copyright law, except executing it on a\r\ncomputer or modifying a private copy.  Propagation includes copying,\r\ndistribution (with or without modification), making available to the\r\npublic, and in some countries other activities as well.\r\n\r\n  To \"convey\" a work means any kind of propagation that enables other\r\nparties to make or receive copies.  Mere interaction with a user through\r\na computer network, with no transfer of a copy, is not conveying.\r\n\r\n  An interactive user interface displays \"Appropriate Legal Notices\"\r\nto the extent that it includes a convenient and prominently visible\r\nfeature that (1) displays an appropriate copyright notice, and (2)\r\ntells the user that there is no warranty for the work (except to the\r\nextent that warranties are provided), that licensees may convey the\r\nwork under this License, and how to view a copy of this License.  If\r\nthe interface presents a list of user commands or options, such as a\r\nmenu, a prominent item in the list meets this criterion.\r\n\r\n  1. Source Code.\r\n\r\n  The \"source code\" for a work means the preferred form of the work\r\nfor making modifications to it.  \"Object code\" means any non-source\r\nform of a work.\r\n\r\n  A \"Standard Interface\" means an interface that either is an official\r\nstandard defined by a recognized standards body, or, in the case of\r\ninterfaces specified for a particular programming language, one that\r\nis widely used among developers working in that language.\r\n\r\n  The \"System Libraries\" of an executable work include anything, other\r\nthan the work as a whole, that (a) is included in the normal form of\r\npackaging a Major Component, but which is not part of that Major\r\nComponent, and (b) serves only to enable use of the work with that\r\nMajor Component, or to implement a Standard Interface for which an\r\nimplementation is available to the public in source code form.  A\r\n\"Major Component\", in this context, means a major essential component\r\n(kernel, window system, and so on) of the specific operating system\r\n(if any) on which the executable work runs, or a compiler used to\r\nproduce the work, or an object code interpreter used to run it.\r\n\r\n  The \"Corresponding Source\" for a work in object code form means all\r\nthe source code needed to generate, install, and (for an executable\r\nwork) run the object code and to modify the work, including scripts to\r\ncontrol those activities.  However, it does not include the work's\r\nSystem Libraries, or general-purpose tools or generally available free\r\nprograms which are used unmodified in performing those activities but\r\nwhich are not part of the work.  For example, Corresponding Source\r\nincludes interface definition files associated with source files for\r\nthe work, and the source code for shared libraries and dynamically\r\nlinked subprograms that the work is specifically designed to require,\r\nsuch as by intimate data communication or control flow between those\r\nsubprograms and other parts of the work.\r\n\r\n  The Corresponding Source need not include anything that users\r\ncan regenerate automatically from other parts of the Corresponding\r\nSource.\r\n\r\n  The Corresponding Source for a work in source code form is that\r\nsame work.\r\n\r\n  2. Basic Permissions.\r\n\r\n  All rights granted under this License are granted for the term of\r\ncopyright on the Program, and are irrevocable provided the stated\r\nconditions are met.  This License explicitly affirms your unlimited\r\npermission to run the unmodified Program.  The output from running a\r\ncovered work is covered by this License only if the output, given its\r\ncontent, constitutes a covered work.  This License acknowledges your\r\nrights of fair use or other equivalent, as provided by copyright law.\r\n\r\n  You may make, run and propagate covered works that you do not\r\nconvey, without conditions so long as your license otherwise remains\r\nin force.  You may convey covered works to others for the sole purpose\r\nof having them make modifications exclusively for you, or provide you\r\nwith facilities for running those works, provided that you comply with\r\nthe terms of this License in conveying all material for which you do\r\nnot control copyright.  Those thus making or running the covered works\r\nfor you must do so exclusively on your behalf, under your direction\r\nand control, on terms that prohibit them from making any copies of\r\nyour copyrighted material outside their relationship with you.\r\n\r\n  Conveying under any other circumstances is permitted solely under\r\nthe conditions stated below.  Sublicensing is not allowed; section 10\r\nmakes it unnecessary.\r\n\r\n  3. Protecting Users' Legal Rights From Anti-Circumvention Law.\r\n\r\n  No covered work shall be deemed part of an effective technological\r\nmeasure under any applicable law fulfilling obligations under article\r\n11 of the WIPO copyright treaty adopted on 20 December 1996, or\r\nsimilar laws prohibiting or restricting circumvention of such\r\nmeasures.\r\n\r\n  When you convey a covered work, you waive any legal power to forbid\r\ncircumvention of technological measures to the extent such circumvention\r\nis effected by exercising rights under this License with respect to\r\nthe covered work, and you disclaim any intention to limit operation or\r\nmodification of the work as a means of enforcing, against the work's\r\nusers, your or third parties' legal rights to forbid circumvention of\r\ntechnological measures.\r\n\r\n  4. Conveying Verbatim Copies.\r\n\r\n  You may convey verbatim copies of the Program's source code as you\r\nreceive it, in any medium, provided that you conspicuously and\r\nappropriately publish on each copy an appropriate copyright notice;\r\nkeep intact all notices stating that this License and any\r\nnon-permissive terms added in accord with section 7 apply to the code;\r\nkeep intact all notices of the absence of any warranty; and give all\r\nrecipients a copy of this License along with the Program.\r\n\r\n  You may charge any price or no price for each copy that you convey,\r\nand you may offer support or warranty protection for a fee.\r\n\r\n  5. Conveying Modified Source Versions.\r\n\r\n  You may convey a work based on the Program, or the modifications to\r\nproduce it from the Program, in the form of source code under the\r\nterms of section 4, provided that you also meet all of these conditions:\r\n\r\n    a) The work must carry prominent notices stating that you modified\r\n    it, and giving a relevant date.\r\n\r\n    b) The work must carry prominent notices stating that it is\r\n    released under this License and any conditions added under section\r\n    7.  This requirement modifies the requirement in section 4 to\r\n    \"keep intact all notices\".\r\n\r\n    c) You must license the entire work, as a whole, under this\r\n    License to anyone who comes into possession of a copy.  This\r\n    License will therefore apply, along with any applicable section 7\r\n    additional terms, to the whole of the work, and all its parts,\r\n    regardless of how they are packaged.  This License gives no\r\n    permission to license the work in any other way, but it does not\r\n    invalidate such permission if you have separately received it.\r\n\r\n    d) If the work has interactive user interfaces, each must display\r\n    Appropriate Legal Notices; however, if the Program has interactive\r\n    interfaces that do not display Appropriate Legal Notices, your\r\n    work need not make them do so.\r\n\r\n  A compilation of a covered work with other separate and independent\r\nworks, which are not by their nature extensions of the covered work,\r\nand which are not combined with it such as to form a larger program,\r\nin or on a volume of a storage or distribution medium, is called an\r\n\"aggregate\" if the compilation and its resulting copyright are not\r\nused to limit the access or legal rights of the compilation's users\r\nbeyond what the individual works permit.  Inclusion of a covered work\r\nin an aggregate does not cause this License to apply to the other\r\nparts of the aggregate.\r\n\r\n  6. Conveying Non-Source Forms.\r\n\r\n  You may convey a covered work in object code form under the terms\r\nof sections 4 and 5, provided that you also convey the\r\nmachine-readable Corresponding Source under the terms of this License,\r\nin one of these ways:\r\n\r\n    a) Convey the object code in, or embodied in, a physical product\r\n    (including a physical distribution medium), accompanied by the\r\n    Corresponding Source fixed on a durable physical medium\r\n    customarily used for software interchange.\r\n\r\n    b) Convey the object code in, or embodied in, a physical product\r\n    (including a physical distribution medium), accompanied by a\r\n    written offer, valid for at least three years and valid for as\r\n    long as you offer spare parts or customer support for that product\r\n    model, to give anyone who possesses the object code either (1) a\r\n    copy of the Corresponding Source for all the software in the\r\n    product that is covered by this License, on a durable physical\r\n    medium customarily used for software interchange, for a price no\r\n    more than your reasonable cost of physically performing this\r\n    conveying of source, or (2) access to copy the\r\n    Corresponding Source from a network server at no charge.\r\n\r\n    c) Convey individual copies of the object code with a copy of the\r\n    written offer to provide the Corresponding Source.  This\r\n    alternative is allowed only occasionally and noncommercially, and\r\n    only if you received the object code with such an offer, in accord\r\n    with subsection 6b.\r\n\r\n    d) Convey the object code by offering access from a designated\r\n    place (gratis or for a charge), and offer equivalent access to the\r\n    Corresponding Source in the same way through the same place at no\r\n    further charge.  You need not require recipients to copy the\r\n    Corresponding Source along with the object code.  If the place to\r\n    copy the object code is a network server, the Corresponding Source\r\n    may be on a different server (operated by you or a third party)\r\n    that supports equivalent copying facilities, provided you maintain\r\n    clear directions next to the object code saying where to find the\r\n    Corresponding Source.  Regardless of what server hosts the\r\n    Corresponding Source, you remain obligated to ensure that it is\r\n    available for as long as needed to satisfy these requirements.\r\n\r\n    e) Convey the object code using peer-to-peer transmission, provided\r\n    you inform other peers where the object code and Corresponding\r\n    Source of the work are being offered to the general public at no\r\n    charge under subsection 6d.\r\n\r\n  A separable portion of the object code, whose source code is excluded\r\nfrom the Corresponding Source as a System Library, need not be\r\nincluded in conveying the object code work.\r\n\r\n  A \"User Product\" is either (1) a \"consumer product\", which means any\r\ntangible personal property which is normally used for personal, family,\r\nor household purposes, or (2) anything designed or sold for incorporation\r\ninto a dwelling.  In determining whether a product is a consumer product,\r\ndoubtful cases shall be resolved in favor of coverage.  For a particular\r\nproduct received by a particular user, \"normally used\" refers to a\r\ntypical or common use of that class of product, regardless of the status\r\nof the particular user or of the way in which the particular user\r\nactually uses, or expects or is expected to use, the product.  A product\r\nis a consumer product regardless of whether the product has substantial\r\ncommercial, industrial or non-consumer uses, unless such uses represent\r\nthe only significant mode of use of the product.\r\n\r\n  \"Installation Information\" for a User Product means any methods,\r\nprocedures, authorization keys, or other information required to install\r\nand execute modified versions of a covered work in that User Product from\r\na modified version of its Corresponding Source.  The information must\r\nsuffice to ensure that the continued functioning of the modified object\r\ncode is in no case prevented or interfered with solely because\r\nmodification has been made.\r\n\r\n  If you convey an object code work under this section in, or with, or\r\nspecifically for use in, a User Product, and the conveying occurs as\r\npart of a transaction in which the right of possession and use of the\r\nUser Product is transferred to the recipient in perpetuity or for a\r\nfixed term (regardless of how the transaction is characterized), the\r\nCorresponding Source conveyed under this section must be accompanied\r\nby the Installation Information.  But this requirement does not apply\r\nif neither you nor any third party retains the ability to install\r\nmodified object code on the User Product (for example, the work has\r\nbeen installed in ROM).\r\n\r\n  The requirement to provide Installation Information does not include a\r\nrequirement to continue to provide support service, warranty, or updates\r\nfor a work that has been modified or installed by the recipient, or for\r\nthe User Product in which it has been modified or installed.  Access to a\r\nnetwork may be denied when the modification itself materially and\r\nadversely affects the operation of the network or violates the rules and\r\nprotocols for communication across the network.\r\n\r\n  Corresponding Source conveyed, and Installation Information provided,\r\nin accord with this section must be in a format that is publicly\r\ndocumented (and with an implementation available to the public in\r\nsource code form), and must require no special password or key for\r\nunpacking, reading or copying.\r\n\r\n  7. Additional Terms.\r\n\r\n  \"Additional permissions\" are terms that supplement the terms of this\r\nLicense by making exceptions from one or more of its conditions.\r\nAdditional permissions that are applicable to the entire Program shall\r\nbe treated as though they were included in this License, to the extent\r\nthat they are valid under applicable law.  If additional permissions\r\napply only to part of the Program, that part may be used separately\r\nunder those permissions, but the entire Program remains governed by\r\nthis License without regard to the additional permissions.\r\n\r\n  When you convey a copy of a covered work, you may at your option\r\nremove any additional permissions from that copy, or from any part of\r\nit.  (Additional permissions may be written to require their own\r\nremoval in certain cases when you modify the work.)  You may place\r\nadditional permissions on material, added by you to a covered work,\r\nfor which you have or can give appropriate copyright permission.\r\n\r\n  Notwithstanding any other provision of this License, for material you\r\nadd to a covered work, you may (if authorized by the copyright holders of\r\nthat material) supplement the terms of this License with terms:\r\n\r\n    a) Disclaiming warranty or limiting liability differently from the\r\n    terms of sections 15 and 16 of this License; or\r\n\r\n    b) Requiring preservation of specified reasonable legal notices or\r\n    author attributions in that material or in the Appropriate Legal\r\n    Notices displayed by works containing it; or\r\n\r\n    c) Prohibiting misrepresentation of the origin of that material, or\r\n    requiring that modified versions of such material be marked in\r\n    reasonable ways as different from the original version; or\r\n\r\n    d) Limiting the use for publicity purposes of names of licensors or\r\n    authors of the material; or\r\n\r\n    e) Declining to grant rights under trademark law for use of some\r\n    trade names, trademarks, or service marks; or\r\n\r\n    f) Requiring indemnification of licensors and authors of that\r\n    material by anyone who conveys the material (or modified versions of\r\n    it) with contractual assumptions of liability to the recipient, for\r\n    any liability that these contractual assumptions directly impose on\r\n    those licensors and authors.\r\n\r\n  All other non-permissive additional terms are considered \"further\r\nrestrictions\" within the meaning of section 10.  If the Program as you\r\nreceived it, or any part of it, contains a notice stating that it is\r\ngoverned by this License along with a term that is a further\r\nrestriction, you may remove that term.  If a license document contains\r\na further restriction but permits relicensing or conveying under this\r\nLicense, you may add to a covered work material governed by the terms\r\nof that license document, provided that the further restriction does\r\nnot survive such relicensing or conveying.\r\n\r\n  If you add terms to a covered work in accord with this section, you\r\nmust place, in the relevant source files, a statement of the\r\nadditional terms that apply to those files, or a notice indicating\r\nwhere to find the applicable terms.\r\n\r\n  Additional terms, permissive or non-permissive, may be stated in the\r\nform of a separately written license, or stated as exceptions;\r\nthe above requirements apply either way.\r\n\r\n  8. Termination.\r\n\r\n  You may not propagate or modify a covered work except as expressly\r\nprovided under this License.  Any attempt otherwise to propagate or\r\nmodify it is void, and will automatically terminate your rights under\r\nthis License (including any patent licenses granted under the third\r\nparagraph of section 11).\r\n\r\n  However, if you cease all violation of this License, then your\r\nlicense from a particular copyright holder is reinstated (a)\r\nprovisionally, unless and until the copyright holder explicitly and\r\nfinally terminates your license, and (b) permanently, if the copyright\r\nholder fails to notify you of the violation by some reasonable means\r\nprior to 60 days after the cessation.\r\n\r\n  Moreover, your license from a particular copyright holder is\r\nreinstated permanently if the copyright holder notifies you of the\r\nviolation by some reasonable means, this is the first time you have\r\nreceived notice of violation of this License (for any work) from that\r\ncopyright holder, and you cure the violation prior to 30 days after\r\nyour receipt of the notice.\r\n\r\n  Termination of your rights under this section does not terminate the\r\nlicenses of parties who have received copies or rights from you under\r\nthis License.  If your rights have been terminated and not permanently\r\nreinstated, you do not qualify to receive new licenses for the same\r\nmaterial under section 10.\r\n\r\n  9. Acceptance Not Required for Having Copies.\r\n\r\n  You are not required to accept this License in order to receive or\r\nrun a copy of the Program.  Ancillary propagation of a covered work\r\noccurring solely as a consequence of using peer-to-peer transmission\r\nto receive a copy likewise does not require acceptance.  However,\r\nnothing other than this License grants you permission to propagate or\r\nmodify any covered work.  These actions infringe copyright if you do\r\nnot accept this License.  Therefore, by modifying or propagating a\r\ncovered work, you indicate your acceptance of this License to do so.\r\n\r\n  10. Automatic Licensing of Downstream Recipients.\r\n\r\n  Each time you convey a covered work, the recipient automatically\r\nreceives a license from the original licensors, to run, modify and\r\npropagate that work, subject to this License.  You are not responsible\r\nfor enforcing compliance by third parties with this License.\r\n\r\n  An \"entity transaction\" is a transaction transferring control of an\r\norganization, or substantially all assets of one, or subdividing an\r\norganization, or merging organizations.  If propagation of a covered\r\nwork results from an entity transaction, each party to that\r\ntransaction who receives a copy of the work also receives whatever\r\nlicenses to the work the party's predecessor in interest had or could\r\ngive under the previous paragraph, plus a right to possession of the\r\nCorresponding Source of the work from the predecessor in interest, if\r\nthe predecessor has it or can get it with reasonable efforts.\r\n\r\n  You may not impose any further restrictions on the exercise of the\r\nrights granted or affirmed under this License.  For example, you may\r\nnot impose a license fee, royalty, or other charge for exercise of\r\nrights granted under this License, and you may not initiate litigation\r\n(including a cross-claim or counterclaim in a lawsuit) alleging that\r\nany patent claim is infringed by making, using, selling, offering for\r\nsale, or importing the Program or any portion of it.\r\n\r\n  11. Patents.\r\n\r\n  A \"contributor\" is a copyright holder who authorizes use under this\r\nLicense of the Program or a work on which the Program is based.  The\r\nwork thus licensed is called the contributor's \"contributor version\".\r\n\r\n  A contributor's \"essential patent claims\" are all patent claims\r\nowned or controlled by the contributor, whether already acquired or\r\nhereafter acquired, that would be infringed by some manner, permitted\r\nby this License, of making, using, or selling its contributor version,\r\nbut do not include claims that would be infringed only as a\r\nconsequence of further modification of the contributor version.  For\r\npurposes of this definition, \"control\" includes the right to grant\r\npatent sublicenses in a manner consistent with the requirements of\r\nthis License.\r\n\r\n  Each contributor grants you a non-exclusive, worldwide, royalty-free\r\npatent license under the contributor's essential patent claims, to\r\nmake, use, sell, offer for sale, import and otherwise run, modify and\r\npropagate the contents of its contributor version.\r\n\r\n  In the following three paragraphs, a \"patent license\" is any express\r\nagreement or commitment, however denominated, not to enforce a patent\r\n(such as an express permission to practice a patent or covenant not to\r\nsue for patent infringement).  To \"grant\" such a patent license to a\r\nparty means to make such an agreement or commitment not to enforce a\r\npatent against the party.\r\n\r\n  If you convey a covered work, knowingly relying on a patent license,\r\nand the Corresponding Source of the work is not available for anyone\r\nto copy, free of charge and under the terms of this License, through a\r\npublicly available network server or other readily accessible means,\r\nthen you must either (1) cause the Corresponding Source to be so\r\navailable, or (2) arrange to deprive yourself of the benefit of the\r\npatent license for this particular work, or (3) arrange, in a manner\r\nconsistent with the requirements of this License, to extend the patent\r\nlicense to downstream recipients.  \"Knowingly relying\" means you have\r\nactual knowledge that, but for the patent license, your conveying the\r\ncovered work in a country, or your recipient's use of the covered work\r\nin a country, would infringe one or more identifiable patents in that\r\ncountry that you have reason to believe are valid.\r\n\r\n  If, pursuant to or in connection with a single transaction or\r\narrangement, you convey, or propagate by procuring conveyance of, a\r\ncovered work, and grant a patent license to some of the parties\r\nreceiving the covered work authorizing them to use, propagate, modify\r\nor convey a specific copy of the covered work, then the patent license\r\nyou grant is automatically extended to all recipients of the covered\r\nwork and works based on it.\r\n\r\n  A patent license is \"discriminatory\" if it does not include within\r\nthe scope of its coverage, prohibits the exercise of, or is\r\nconditioned on the non-exercise of one or more of the rights that are\r\nspecifically granted under this License.  You may not convey a covered\r\nwork if you are a party to an arrangement with a third party that is\r\nin the business of distributing software, under which you make payment\r\nto the third party based on the extent of your activity of conveying\r\nthe work, and under which the third party grants, to any of the\r\nparties who would receive the covered work from you, a discriminatory\r\npatent license (a) in connection with copies of the covered work\r\nconveyed by you (or copies made from those copies), or (b) primarily\r\nfor and in connection with specific products or compilations that\r\ncontain the covered work, unless you entered into that arrangement,\r\nor that patent license was granted, prior to 28 March 2007.\r\n\r\n  Nothing in this License shall be construed as excluding or limiting\r\nany implied license or other defenses to infringement that may\r\notherwise be available to you under applicable patent law.\r\n\r\n  12. No Surrender of Others' Freedom.\r\n\r\n  If conditions are imposed on you (whether by court order, agreement or\r\notherwise) that contradict the conditions of this License, they do not\r\nexcuse you from the conditions of this License.  If you cannot convey a\r\ncovered work so as to satisfy simultaneously your obligations under this\r\nLicense and any other pertinent obligations, then as a consequence you may\r\nnot convey it at all.  For example, if you agree to terms that obligate you\r\nto collect a royalty for further conveying from those to whom you convey\r\nthe Program, the only way you could satisfy both those terms and this\r\nLicense would be to refrain entirely from conveying the Program.\r\n\r\n  13. Use with the GNU Affero General Public License.\r\n\r\n  Notwithstanding any other provision of this License, you have\r\npermission to link or combine any covered work with a work licensed\r\nunder version 3 of the GNU Affero General Public License into a single\r\ncombined work, and to convey the resulting work.  The terms of this\r\nLicense will continue to apply to the part which is the covered work,\r\nbut the special requirements of the GNU Affero General Public License,\r\nsection 13, concerning interaction through a network will apply to the\r\ncombination as such.\r\n\r\n  14. Revised Versions of this License.\r\n\r\n  The Free Software Foundation may publish revised and/or new versions of\r\nthe GNU General Public License from time to time.  Such new versions will\r\nbe similar in spirit to the present version, but may differ in detail to\r\naddress new problems or concerns.\r\n\r\n  Each version is given a distinguishing version number.  If the\r\nProgram specifies that a certain numbered version of the GNU General\r\nPublic License \"or any later version\" applies to it, you have the\r\noption of following the terms and conditions either of that numbered\r\nversion or of any later version published by the Free Software\r\nFoundation.  If the Program does not specify a version number of the\r\nGNU General Public License, you may choose any version ever published\r\nby the Free Software Foundation.\r\n\r\n  If the Program specifies that a proxy can decide which future\r\nversions of the GNU General Public License can be used, that proxy's\r\npublic statement of acceptance of a version permanently authorizes you\r\nto choose that version for the Program.\r\n\r\n  Later license versions may give you additional or different\r\npermissions.  However, no additional obligations are imposed on any\r\nauthor or copyright holder as a result of your choosing to follow a\r\nlater version.\r\n\r\n  15. Disclaimer of Warranty.\r\n\r\n  THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY\r\nAPPLICABLE LAW.  EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT\r\nHOLDERS AND/OR OTHER PARTIES PROVIDE THE PROGRAM \"AS IS\" WITHOUT WARRANTY\r\nOF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO,\r\nTHE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR\r\nPURPOSE.  THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM\r\nIS WITH YOU.  SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF\r\nALL NECESSARY SERVICING, REPAIR OR CORRECTION.\r\n\r\n  16. Limitation of Liability.\r\n\r\n  IN NO EVENT UNLESS REQUIRED BY APPLICABLE LAW OR AGREED TO IN WRITING\r\nWILL ANY COPYRIGHT HOLDER, OR ANY OTHER PARTY WHO MODIFIES AND/OR CONVEYS\r\nTHE PROGRAM AS PERMITTED ABOVE, BE LIABLE TO YOU FOR DAMAGES, INCLUDING ANY\r\nGENERAL, SPECIAL, INCIDENTAL OR CONSEQUENTIAL DAMAGES ARISING OUT OF THE\r\nUSE OR INABILITY TO USE THE PROGRAM (INCLUDING BUT NOT LIMITED TO LOSS OF\r\nDATA OR DATA BEING RENDERED INACCURATE OR LOSSES SUSTAINED BY YOU OR THIRD\r\nPARTIES OR A FAILURE OF THE PROGRAM TO OPERATE WITH ANY OTHER PROGRAMS),\r\nEVEN IF SUCH HOLDER OR OTHER PARTY HAS BEEN ADVISED OF THE POSSIBILITY OF\r\nSUCH DAMAGES.\r\n\r\n  17. Interpretation of Sections 15 and 16.\r\n\r\n  If the disclaimer of warranty and limitation of liability provided\r\nabove cannot be given local legal effect according to their terms,\r\nreviewing courts shall apply local law that most closely approximates\r\nan absolute waiver of all civil liability in connection with the\r\nProgram, unless a warranty or assumption of liability accompanies a\r\ncopy of the Program in return for a fee.\r\n\r\n                     END OF TERMS AND CONDITIONS");
        }

        /// <summary>
        /// Output the warranty disclaimer per the GPL 3.0 license
        /// <para> See <see cref="GetCommand(string[])"/> </para>
        /// </summary>
        static void Warranty()
        {
            Console.WriteLine("THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY\nAPPLICABLE LAW.  EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT\nHOLDERS AND/OR OTHER PARTIES PROVIDE THE PROGRAM \"AS IS\" WITHOUT WARRANTY\nOF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO,\nTHE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR\nPURPOSE.  THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM\nIS WITH YOU.  SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF\nALL NECESSARY SERVICING, REPAIR OR CORRECTION.");
        }

        /// <summary>
        /// Show authorship and license information regarding Pigmeat
        /// <para> See <see cref="GetCommand(string[])"/> </para>
        /// </summary>
        static void About()
        {
            Console.WriteLine("Copyright (C) 2020 Emil Sayahi\nThis program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, version 3.\n\nThis program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.\n\nYou should have received a copy of the GNU General Public License along with this program. If not, see <https://www.gnu.org/licenses/>.");
        }

        /// <summary>
        /// The standard Pigmeat build process
        /// <para> See <see cref="GetCommand(string[])"/> </para>
        /// <seealso cref="ServeWatch"/>
        /// </summary>
        static void Build(bool CleanCollections)
        {
            int i = 0; // Keep track of files rendered
            JObject Global = JObject.Parse(IO.GetGlobal());
            string[] IncludedFiles = JsonConvert.DeserializeObject<string[]>(Global["included-files"].ToString());
            string[] IncludedPages = JsonConvert.DeserializeObject<string[]>(Global["included-pages"].ToString());
            foreach(var layout in Directory.GetFiles("./layouts", "*", SearchOption.TopDirectoryOnly))
            {
                IO.GetLayoutContents(layout, false);
            }
            foreach(var directory in Directory.GetDirectories("./", "_*", SearchOption.TopDirectoryOnly))
            {
                foreach(var file in Directory.GetFiles(directory, "*", SearchOption.TopDirectoryOnly))
                {
                    if(Path.GetExtension(file).Equals(".md") || Path.GetExtension(file).Equals(".html"))
                    {
                        JObject PageObject = Page.GetPageObject(file);
                        Directory.CreateDirectory(Path.GetDirectoryName("./output/" + PageObject["url"].ToString()));
                        if(PageObject.ContainsKey("paginate"))
                        {
                            Paginator.RenderPaginated(file); // Output multiple pages
                        }
                        else
                        {
                            File.WriteAllText("./output/" + PageObject["url"].ToString(), PageObject["content"].ToString());
                        }
                        //Console.WriteLine(file + " → " + "./output/" + PageObject["url"].ToString());
                        i++;
                    }
                    else if(Path.GetExtension(file).Equals(".scss") || Path.GetExtension(file).Equals(".sass"))
                    {
                        Directory.CreateDirectory("./output/" + Path.GetDirectoryName(file));
                        File.WriteAllText("./output/" + Path.GetDirectoryName(file) + "/" + Path.GetFileNameWithoutExtension(file) + ".css", Scss.ConvertToCss(File.ReadAllText(file)).Css);
                        //Console.WriteLine(file + " → " + "./output/" + Path.GetDirectoryName(file) + "/" + Path.GetFileNameWithoutExtension(file) + ".css";
                        i++;
                    }
                    else if(!Path.GetExtension(file).Equals(".json") && !Path.GetExtension(file).Equals(".yml"))
                    {
                        Directory.CreateDirectory("./output/" + Path.GetDirectoryName(file));
                        File.Copy(file, "./output/" + file, true);
                        //Console.WriteLine(file + " → " + "./output/" + file);
                        i++;
                    }
                }
            }
            try
            {
                foreach(var file in IncludedFiles)
                {
                    Directory.CreateDirectory("./output/" + Path.GetDirectoryName(file));
                    try
                    {
                        File.Copy(file, "./output/" + file, true);
                    }
                    catch(IOException)
                    {
                        IO.IncludeDirectory(file, "./output/" + file);
                        //Console.WriteLine(file + " → " + "./output/" + file);
                        i++;
                    }
                    //Console.WriteLine(file + " → " + "./output/" + file);
                    i++;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            try
            {
                foreach(var file in IncludedPages)
                {
                    JObject PageObject = Page.GetPageObject(file);
                    
                    if(PageObject.ContainsKey("paginate"))
                    {
                        Paginator.RenderPaginated(file);
                    }
                    else
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName("./output/" + PageObject["url"].ToString()));
                        File.WriteAllText("./output/" + PageObject["url"].ToString(), PageObject["content"].ToString());
                    }
                    //Console.WriteLine(file + " → " + "./output/" + PageObject["url"].ToString());
                    i++;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine(i + " document(s) processed.");

            if(CleanCollections)
            {
                IO.CleanCollections();
                // Cannot make this check with IO.Serving as then we'd end up with wiped collections regardless (wipe happens before IO.Serving is set)
            }
            IO.Serving = true;
        }

        /// <summary>
        /// Create a barebones Pigmeat project
        /// <para> See <see cref="GetCommand(string[])"/> </para>
        /// </summary>
        static void New()
        {
            Directory.CreateDirectory("./_posts");
            Directory.CreateDirectory("./drafts");
            Directory.CreateDirectory("./layouts");
            Directory.CreateDirectory("./snippets");
            Directory.CreateDirectory("./sass");
            Directory.CreateDirectory("./plugins");
            Directory.CreateDirectory("./scripts");

            File.WriteAllText("./_posts/collection.json", "{\n\t\"name\": \"posts\",\n\t\"entries\": []\n}");
            File.WriteAllText("./_posts/README.md", "---\r\ntitle: \"A post\"\r\npermalink: \"index.html\"\r\n---\r\nThis is a post, generated from `README.md` in the `posts` folder.");
            File.WriteAllText("./_global.yml", "title: Pigmeat Project\nculture: \"en-US\"\nincluded-files: []\nincluded-pages: []");
            File.WriteAllText("./drafts/README", "This is where your Markdown and HTML documents should go if you don't want them to be published.");
            File.WriteAllText("./layouts/README", "This is where your HTML page templates go.");
            File.WriteAllText("./snippets/README", "This is where your HTML snippets go.");
            File.WriteAllText("./sass/README", "This is where your Sass stylesheet dependencies go.");
            File.WriteAllText("./plugins/README", "This is where your '.ham,' compiled plugins go, along with their dependencies.");
            File.WriteAllText("./scripts/README", "This is where your '.cs' scripts, along with their dependencies, go.");
        }

        /// <summary>
        /// Clean the Pigmeat <c>output</c> directory
        /// <para> See <see cref="GetCommand(string[])"/> </para>
        /// </summary>
        static void Clean()
        {
            IO.CleanCollections();
            try
            {
                Directory.Delete("./output", true);
                Console.WriteLine("Cleaned project directory … ");
            }
            catch(DirectoryNotFoundException)
            {
                // This is expected if there is no directory to clean.
                Console.WriteLine("Nothing to clean … ");
            }
        }

        /// <summary>
        /// Watch the file system for changes (initial component of the <c>serve</c> command)
        /// <para> See <see cref="GetCommand(string[])"/> </para>
        /// <seealso cref="ServeBuild"/>
        /// </summary>
        static void ServeWatch()
        {
            Console.WriteLine("Serving is intended for development purposes only. Use 'build' for production situations. Remember to use 'clean' when you exit.\n\nPerforming initial build … ");
            Build(false); // Required to collect initial build data (as opposed to doing 'build' then 'serve' commands)
            // Build.CleanCollections must be false so collection entries are not wiped

            FileSystemWatcher Watcher = new FileSystemWatcher() { Path = "./", IncludeSubdirectories = true, Filter = "*"};
            Watcher.NotifyFilter = NotifyFilters.LastWrite
                                 | NotifyFilters.FileName
                                 | NotifyFilters.DirectoryName;
            Watcher.Changed += new FileSystemEventHandler(ServeBuild);
            Watcher.Created += new FileSystemEventHandler(ServeBuild);
            Watcher.Deleted += new FileSystemEventHandler(ServeBuild);
            Watcher.EnableRaisingEvents = true; // Begins the watching loop
            while(true);
        }

        /// <summary>
        /// Rebuild changed files (final component of the <c>serve</c> command)
        /// </summary>
        static void ServeBuild(object source, FileSystemEventArgs e)
        {
            var RelativePath = Path.GetRelativePath("./", e.FullPath); // Relative paths are nicer to work with
            /*
            Filter out the following situations:
                1. Changes by Pigmeat iself
                2. Changes recorded by git
                3. Changes to directories (no purpose in doing anything when this occurs)
                4. Changes made mere milliseconds apart
            */
            if(File.GetAttributes(e.FullPath).HasFlag(FileAttributes.Directory) || Path.GetFileName(RelativePath).Equals("_global.yml") || RelativePath.Contains("output/") || RelativePath.Contains(".git/") || DateTime.Now.Subtract(LastFileWatcherEvent).TotalMilliseconds < 500)
            {
                return;
            }
            LastFileWatcherEvent = DateTime.Now; // Keeps track of when the last proper event occured
        
            Console.WriteLine("\n" + RelativePath + " has been modified. Rebuilding … ");

            JObject Global = JObject.Parse(IO.GetGlobal());
            string[] IncludedFiles = JsonConvert.DeserializeObject<string[]>(Global["included-files"].ToString());
            string[] IncludedPages = JsonConvert.DeserializeObject<string[]>(Global["included-pages"].ToString());

            // If a layout is changed, overwrite it in the cache and rebuild its dependencies
            if(Path.GetDirectoryName(RelativePath).Equals("layout"))
            {
                IO.GetLayoutContents(RelativePath, true);
                foreach(var file in Directory.GetFiles("./", "*", SearchOption.TopDirectoryOnly))
                {
                    if(Path.GetExtension(RelativePath).Equals(".md") || Path.GetExtension(RelativePath).Equals(".html"))
                    {
                        JObject PageObject = Page.GetPageObject(RelativePath);
                        if(PageObject["layout"].ToString().Equals(Path.GetFileNameWithoutExtension(RelativePath)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName("./output/" + PageObject["url"].ToString()));
                            File.WriteAllText("./output/" + PageObject["url"].ToString(), PageObject["content"].ToString());
                        }
                    }
                }
            }
            // If a snippet is changed, rebuild its dependencies
            else if(Path.GetDirectoryName(RelativePath).Equals("snippets"))
            {
                foreach(var file in Directory.GetFiles("./", "*", SearchOption.TopDirectoryOnly))
                {
                    if(Path.GetExtension(RelativePath).Equals(".md") || Path.GetExtension(RelativePath).Equals(".html"))
                    {
                        if(File.ReadAllText(file).Contains("{! snippet " + Path.GetFileName(RelativePath)))
                        {
                            JObject PageObject = Page.GetPageObject(RelativePath);
                            Directory.CreateDirectory(Path.GetDirectoryName("./output/" + PageObject["url"].ToString()));
                            File.WriteAllText("./output/" + PageObject["url"].ToString(), PageObject["content"].ToString());
                        }
                    }
                }
            }
            // If a document is changed, rebuild it
            else if(Path.GetExtension(RelativePath).Equals(".md") || Path.GetExtension(RelativePath).Equals(".html"))
            {
                JObject PageObject = Page.GetPageObject(RelativePath);
                Directory.CreateDirectory(Path.GetDirectoryName("./output/" + PageObject["url"].ToString()));
                File.WriteAllText("./output/" + PageObject["url"].ToString(), PageObject["content"].ToString());
            }
            // If the stylesheet(s) are changed, rebuild them
            else if(Path.GetExtension(RelativePath).Equals(".scss") || Path.GetExtension(RelativePath).Equals(".sass"))
            {
                Directory.CreateDirectory("./output/" + Path.GetDirectoryName(RelativePath));
                File.WriteAllText("./output/" + Path.GetDirectoryName(RelativePath) + "/" + Path.GetFileNameWithoutExtension(RelativePath) + ".css", Scss.ConvertToCss(File.ReadAllText(RelativePath)).Css);
            }
            // If a file has changed, copy it again
            else if(!Path.GetExtension(RelativePath).Equals(".json") && !Path.GetExtension(RelativePath).Equals(".yml"))
            {
                Directory.CreateDirectory("./output/" + Path.GetDirectoryName(RelativePath));
                File.Copy(RelativePath, "./output/" + RelativePath, true);
            }
            // If 'included' and changed, then re-output
            try
            {
                foreach(var file in IncludedFiles)
                {
                    if(file.Equals(RelativePath))
                    {
                        Directory.CreateDirectory("./output/" + Path.GetDirectoryName(RelativePath));
                        try
                        {
                            File.Copy(RelativePath, "./output/" + RelativePath, true);
                        }
                        catch(IOException)
                        {
                            IO.IncludeDirectory(RelativePath, "./output/" + RelativePath);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            try
            {
                foreach(var file in IncludedPages)
                {
                    if(file.Equals(RelativePath))
                    {
                        JObject PageObject = Page.GetPageObject(RelativePath);
                        Directory.CreateDirectory(Path.GetDirectoryName("./output/" + PageObject["url"].ToString()));
                        File.WriteAllText("./output/" + PageObject["url"].ToString(), PageObject["content"].ToString());
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            return;
        }

        /// <summary>
        /// Install a Pigmeat theme
        /// <para> See <see cref="GetCommand(string[])"/> </para>
        /// </summary>
        static void Install(string[] args)
        {
            var PresentPath = Path.GetFullPath(".");
            var SplitRepositoryCall = args[args.Length - 1].Split(':', StringSplitOptions.RemoveEmptyEntries);
            string OwnerName, RepositoryName;
            switch(SplitRepositoryCall.Length)
            {
                case 2:
                    OwnerName = SplitRepositoryCall[0];
                    RepositoryName = SplitRepositoryCall[1];
                    break;
                default:
                    OwnerName = "MadeByEmil";
                    RepositoryName = SplitRepositoryCall[0];
                    break;
            }

            var GitUrl = "https://github.com/" + OwnerName + "/" + RepositoryName + ".git";

            using(WebClient client = new WebClient())
            {
                try
                {
                    var PackageObject = IO.GetYamlObject(client.DownloadString("https://raw.githubusercontent.com/" + OwnerName + "/" + RepositoryName + "/master/.hampkg"));
                    Console.WriteLine(PackageObject["name"].ToString() + "\n" + PackageObject["description"].ToString() + "\nAuthor(s): " + PackageObject["author"].ToString() + "\nLicense: " + PackageObject["license"].ToString());
                    Console.WriteLine("\nInstall " + PackageObject["type"].ToString() + "?");
                    var Answer = Console.ReadKey(true).Key;
                    if(Answer != ConsoleKey.Y && Answer != ConsoleKey.Enter)
                    {
                        Environment.Exit(77); // Permission denied
                    }
                }
                catch(Exception)
                {
                    Console.WriteLine("Could not retrieve package at " + GitUrl + ". Ensure the package has a '.hampkg' file with required values.");
                    Environment.Exit(66); // Cannot open input
                }
            }
            
            try
            {
                Repository.Clone(GitUrl, "./");
                Console.WriteLine("Installed " + RepositoryName + " to " + PresentPath);
            }
            catch(NameConflictException)
            {
                Console.WriteLine("Everything in " + PresentPath + " will be overwritten. Continue? ");
                var Answer = Console.ReadKey(true).Key;
                
                if(Answer == ConsoleKey.Y || Answer == ConsoleKey.Enter)
                {
                    Directory.Delete("./", true);
                    Directory.CreateDirectory(PresentPath);
                    Repository.Clone(GitUrl, PresentPath);
                    Console.WriteLine("Installed " + RepositoryName + " to " + PresentPath);
                }
            }
            
            File.Delete(PresentPath + "/.hampkg"); // Remove '.hampkg' file from the downloaded package
        }

        /// <summary>
        /// Show how to use the Pigmeat tool
        /// <para> See <see cref="GetCommand(string[])"/> </para>
        /// </summary>
        static void Help(string[] args)
        {
            if(args.Length <= 1)
            {
                Console.WriteLine(
                    "Pigmeat has the following commands:\n" +
                    "    pigmeat new <path:optional> - Creates an empty Pigmeat project.\n" +
                    "    pigmeat build <path:optional> - Outputs a publishable Pigmeat project.\n" +
                    "    pigmeat serve <path:optional> - Continuously rebuilds a Pigmeat project when file changes are made.\n" +
                    "    pigmeat install <path:optional> <owner:optional>:<repo:required> - Installs a Pigmeat package from GitHub.\n" +
                    "    pigmeat clean <path:optional> - Deletes all generated data that results from the build process.\n" +
                    "    pigmeat help <command:optional> - Displays an informational message regarding the usage of Pigmeat."
                    );
                Environment.Exit(0);
            }
            switch (args[1]) {
                case "new":
                case "n":
                    Console.WriteLine("Creates an empty Pigmeat project. A path may be specified, otherwise a project will be created where Pigmeat is running.");
                    break;
                case "build":
                case "b":
                    Console.WriteLine("Outputs a publishable Pigmeat project. A path may be specified, otherwise a project will be built where Pigmeat is running.");
                    break;
                case "serve":
                case "s":
                    Console.WriteLine("Continuously rebuilds a Pigmeat project when file changes are made. Intended for previewing changes during development.");
                    break;
                case "install":
                case "i":
                    Console.WriteLine("Installs a Pigmeat package from GitHub. This command takes the name of the package's repository, along with the account that owns it (e.g. 'pigmeat install MadeByEmil:pigmeat-basic').");
                    break;
                case "clean":
                case "c":
                    Console.WriteLine("Deletes all generated data that results from the build process.");
                    break;
                case "help":
                case "h":
                    Console.WriteLine("Prints a message outlining Pigmeat's commands. A subparameter may be specified, displaying a message outlining the usage of the given parameter (e.g. 'pigmeat help serve').");
                    break;
                case "about":
                    About();
                    break;
                case "warranty":
                case "w":
                    Warranty();
                    break;
                case "terms":
                case "t":
                    Terms();
                    break;
                default:
                    Console.WriteLine("Please specify a parameter (e.g. 'pigmeat help new,' 'pigmeat help build,' 'pigmeat help serve,' 'pigmeat help clean').");
                    break;
            }
        }
    }
}