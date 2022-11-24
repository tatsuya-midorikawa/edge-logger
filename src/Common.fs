﻿[<AutoOpen>]
module Common
let inline msgbox'show (msg: string) = System.Windows.Forms.MessageBox.Show (msg, "edge-logger.exe") |> ignore

let license = """EDGE-LOGGER SOFTWARE LICENSE TERMS
These license terms are an agreement between Microsoft Corporation and you.  Please read them. The terms also apply to any edge-logger
·   updates,
·   supplements,
·   Internet-based services, and 
·   support services
for this software, unless other terms accompany those items.  If so, those terms apply.
BY USING THE SOFTWARE, YOU ACCEPT THESE TERMS.  IF YOU DO NOT ACCEPT THEM, DO NOT USE THE SOFTWARE.
If you comply with these license terms, you have the rights below.
1.  INSTALLATION AND USE RIGHTS.  You may install and use any number of copies of the software on your devices.
2.  Scope of License.  The software is licensed, not sold. This agreement only gives you some rights to use the software.  Microsoft reserves all other rights.  Unless applicable law gives you more rights despite this limitation, you may use the software only as expressly permitted in this agreement.  In doing so, you must comply with any technical limitations in the software that only allow you to use it in certain ways.    You may not
·   work around any technical limitations in the binary versions of the software;
·   reverse engineer, decompile or disassemble the binary versions of the software, except and only to the extent that applicable law expressly permits, despite this limitation;
·   make more copies of the software than specified in this agreement or allowed by applicable law, despite this limitation;
·   publish the software for others to copy;
·   rent, lease or lend the software;
·   transfer the software or this agreement to any third party; or
·   use the software for commercial software hosting services.
3.  SENSITIVE INFORMATION.  Please be aware that, similar to other debug tools that capture "process state" information, files saved by edge-logger may include personally identifiable or other sensitive information (such as usernames, passwords, paths to files accessed, and paths to registry accessed). By using this software, you acknowledge that you are aware of this and take sole responsibility for any personally identifiable or other sensitive information provided to Microsoft or any other party through your use of the software.
.   DOCUMENTATION.  Any person that has valid access to your computer or internal network may copy and use the documentation for your internal, reference purposes.
6.  Export Restrictions.  The software is subject to United States export laws and regulations.  You must comply with all domestic and international export laws and regulations that apply to the software.  These laws include restrictions on destinations, end users and end use.  For additional information, see www.microsoft.com/exporting <<http://www.microsoft.com/exporting>>.
7.  SUPPORT SERVICES. Because this software is "as is, " we may not provide support services for it.
8.  Entire Agreement.  This agreement, and the terms for supplements, updates, Internet-based services and support services that you use, are the entire agreement for the software and support services.
9.  Applicable Law.
a.  United States.  If you acquired the software in the United States, Washington state law governs the interpretation of this agreement and applies to claims for breach of it, regardless of conflict of laws principles.  The laws of the state where you live govern all other claims, including claims under state consumer protection laws, unfair competition laws, and in tort.
b.  Outside the United States.  If you acquired the software in any other country, the laws of that country apply.
10. Legal Effect.  This agreement describes certain legal rights.  You may have other rights under the laws of your country.  You may also have rights with respect to the party from whom you acquired the software.  This agreement does not change your rights under the laws of your country if the laws of your country do not permit it to do so.
11. Disclaimer of Warranty.   The software is licensed "as - is."  You bear the risk of using it.  Microsoft gives no express warranties, guarantees or conditions.  You may have additional consumer rights under your local laws which this agreement cannot change.  To the extent permitted under your local laws, Microsoft excludes the implied warranties of merchantability, fitness for a particular purpose and non-infringement.
12. Limitation on and Exclusion of Remedies and Damages.  You can recover from Microsoft and its suppliers only direct damages up to U.S. $5.00.  You cannot recover any other damages, including consequential, lost profits, special, indirect or incidental damages.
This limitation applies to
·   anything related to the software, services, content (including code) on third party Internet sites, or third party programs; and
·   claims for breach of contract, breach of warranty, guarantee or condition, strict liability, negligence, or other tort to the extent permitted by applicable law.
It also applies even if edge-logger knew or should have known about the possibility of the damages.  The above limitation or exclusion may not apply to you because your country may not allow the exclusion or limitation of incidental, consequential or other damages.
"""