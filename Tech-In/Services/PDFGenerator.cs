using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tech_In.Models.ViewModels.ProfileViewModels;

namespace Tech_In.Services
{
    public class PDFGenerator
    {
        #region Declerations
        int _totalColumn = 6;
        Document _document;
        Font _fontStyle;
        PdfPTable _pdfPTable = new PdfPTable(6);
        PdfPCell _pdfPCell;
        MemoryStream _memoryStream = new MemoryStream();
        ProfileViewModal _user;
        //University _university = new University();
        #endregion
        public PDFGenerator(ProfileViewModal pVM)
        {
            _user = pVM;
        }

        public byte[] PrepareReport()
        {
            //_university = university;

            #region
            _document = new Document(PageSize.A4, 50f, 50f, 50f, 50f);
            
            _pdfPTable.WidthPercentage = 100;
            _pdfPTable.HorizontalAlignment = Element.ALIGN_LEFT;
            _fontStyle = FontFactory.GetFont("Arial", 8f, 0);
            PdfWriter.GetInstance(_document, _memoryStream);
            _document.Open();
            _pdfPTable.SetWidths(new float[] { 20f, 25f, 25f, 25f, 25f, 25f });
            #endregion

            this.ReportHeader();
            this.HorizontalLine();
            this.Objective();

            if (_user.ExpVMList.Count() != 0)
                Experience();

            if (_user.EduVMList.Count() != 0)
                this.Education();

            if (_user.CertificationVMList.Count() != 0)
                Certifications();

            if (_user.PublicationVMListJP.Count() != 0)
                JournalP();

            if (_user.PublicationVMListCP.Count() != 0)
                ConferenceP();

            if (_user.AchievVMList.Count() != 0)
                Achievements();

            if (_user.HobbyVMList.Count() != 0)
                Hobbies();

            if (_user.LanguageSkillVMList.Count() != 0)
                LanguageSkills();

            //this.Projects();
            _pdfPTable.HeaderRows = 2;
            _document.Add(_pdfPTable);
            _document.Close();
            return _memoryStream.ToArray();
        }

        private void ReportHeader()
        {
            //Name
            _fontStyle = FontFactory.GetFont("Calibri", 18f, 1);
            _pdfPCell = new PdfPCell(new Phrase(_user.UserPersonalVM.FullName, _fontStyle));
            _pdfPCell.Colspan = _totalColumn;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfPCell.Border = 0;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfPTable.AddCell(_pdfPCell);
            _pdfPTable.CompleteRow();

            ////////Address Row
            //////_fontStyle = FontFactory.GetFont("Calibri", 12f, 1);
            //////_fontStyle.SetColor(25, 111, 61);
            //////_pdfPCell = new PdfPCell(new Phrase("Address: ", _fontStyle));
            //////_pdfPCell.Colspan = 1;
            //////_pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            //////_pdfPCell.Border = 0;
            //////_pdfPCell.BackgroundColor = BaseColor.WHITE;
            //////_pdfPCell.ExtraParagraphSpace = 0;
            //////_pdfPTable.AddCell(_pdfPCell);

            //////_fontStyle = FontFactory.GetFont("Calibri", 11f, 0);
            //////_pdfPCell = new PdfPCell(new Phrase("House 1, Sector 1, Khan Akbar Town, New Shakrial, Islamabad, Pakistan ", _fontStyle));
            //////_pdfPCell.Colspan = 5;
            //////_pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            //////_pdfPCell.VerticalAlignment = Element.ALIGN_BOTTOM;
            //////_pdfPCell.Border = 0;
            //////_pdfPCell.BackgroundColor = BaseColor.WHITE;
            //////_pdfPCell.ExtraParagraphSpace = 0;
            //////_pdfPTable.AddCell(_pdfPCell);


            //_fontStyle = FontFactory.GetFont("Calibri", 8f, 0);
            ////_pdfPCell = new PdfPCell(new Phrase("[Add Image Here]", _fontStyle));
            //_pdfPCell = new PdfPCell(new Phrase("", _fontStyle));
            //_pdfPCell.Colspan = 1;
            //_pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            //_pdfPCell.Border = 0;
            //_pdfPCell.BackgroundColor = BaseColor.WHITE;
            //_pdfPCell.ExtraParagraphSpace = 0;
            //_pdfPTable.AddCell(_pdfPCell);
            _pdfPTable.CompleteRow();

            //Email Row
            _fontStyle = FontFactory.GetFont("Calibri", 12f, 1);
            _fontStyle.SetColor(25, 111, 61);
            _pdfPCell = new PdfPCell(new Phrase("E-mail: ", _fontStyle));
            _pdfPCell.Colspan = 1;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfPCell.Border = 0;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfPTable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Calibri", 11f, 0);
            _pdfPCell = new PdfPCell(new Phrase(_user.UserPersonalVM.Email, _fontStyle));
            _pdfPCell.Colspan = 2;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfPCell.VerticalAlignment = Element.ALIGN_BOTTOM;
            _pdfPCell.Border = 0;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfPTable.AddCell(_pdfPCell);

            if (_user.UserPersonalVM.PhoneNo != null)
            {
                _fontStyle = FontFactory.GetFont("Calibri", 12f, 1);
                _fontStyle.SetColor(25, 111, 61);
                _pdfPCell = new PdfPCell(new Phrase("Mobile #: ", _fontStyle));
                _pdfPCell.Colspan = 1;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfPTable.AddCell(_pdfPCell);

                _fontStyle = FontFactory.GetFont("Calibri", 11f, 0);
                _pdfPCell = new PdfPCell(new Phrase(_user.UserPersonalVM.PhoneNo, _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_BOTTOM;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfPTable.AddCell(_pdfPCell);
            }
            else
            {
                _fontStyle = FontFactory.GetFont("Calibri", 10f, 1);
                _fontStyle.SetColor(25, 111, 61);
                _pdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                _pdfPCell.Colspan = 1;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfPTable.AddCell(_pdfPCell);

                _fontStyle = FontFactory.GetFont("Calibri", 9f, 0);
                _pdfPCell = new PdfPCell(new Phrase("", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.VerticalAlignment = Element.ALIGN_BOTTOM;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfPTable.AddCell(_pdfPCell);
            }
            

            _pdfPTable.CompleteRow();

            _fontStyle = FontFactory.GetFont("Arial", 10f, 0);
            _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
            _pdfPCell.Colspan = _totalColumn;
            _pdfPCell.Border = 0;
            _pdfPTable.AddCell(_pdfPCell);

            _pdfPTable.CompleteRow();



        }

        private void Objective()
        {
            //New Line
            _fontStyle = FontFactory.GetFont("Arial", 10f, 0);
            _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
            _pdfPCell.Colspan = _totalColumn;
            _pdfPCell.Border = 0;
            _pdfPTable.AddCell(_pdfPCell);

            _pdfPTable.CompleteRow();

            _fontStyle = FontFactory.GetFont("Calibri", 14f, 1);
            _fontStyle.SetColor(33, 97, 140);
            _pdfPCell = new PdfPCell(new Phrase("Objective", _fontStyle));
            _pdfPCell.Colspan = _totalColumn;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfPCell.Border = 0;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfPTable.AddCell(_pdfPCell);
            _pdfPTable.CompleteRow();

            _fontStyle = FontFactory.GetFont("Calibri", 12f, 0);
            if(_user.UserPersonalVM.Summary!=null)
                _pdfPCell = new PdfPCell(new Phrase(_user.UserPersonalVM.Summary, _fontStyle));
            else
                _pdfPCell = new PdfPCell(new Phrase("No Summary added yet!", _fontStyle));
            _pdfPCell.Colspan = _totalColumn;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfPCell.Border = 0;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfPTable.AddCell(_pdfPCell);

            _pdfPTable.CompleteRow();
        }

        private void Education()
        {
            _fontStyle = FontFactory.GetFont("Calibri", 14f, 1);
            _fontStyle.SetColor(33, 97, 140);
            _pdfPCell = new PdfPCell(new Phrase("Education", _fontStyle));
            _pdfPCell.Colspan = _totalColumn;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfPCell.Border = 0;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfPTable.AddCell(_pdfPCell);
            _pdfPTable.CompleteRow();

            foreach(var edu in _user.EduVMList)
            {
                _fontStyle = FontFactory.GetFont("Calibri", 13f, 1);
                _fontStyle.SetColor(25, 111, 61);
                _pdfPCell = new PdfPCell(new Phrase(edu.SchoolName, _fontStyle));
                _pdfPCell.Colspan = 4;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfPTable.AddCell(_pdfPCell);

                _fontStyle = FontFactory.GetFont("Calibri", 11f, 0);
                if (edu.CurrentStatusCheck)
                    _pdfPCell = new PdfPCell(new Phrase(edu.StartDate.ToString("dd MMM yyyy") + " - Current", _fontStyle));
                else
                    _pdfPCell = new PdfPCell(new Phrase(edu.StartDate.ToString("dd MMM yyyy") + " - " + edu.EndDate.ToString("dd MMM yyyy"), _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfPTable.AddCell(_pdfPCell);

                _pdfPTable.CompleteRow();

                _fontStyle = FontFactory.GetFont("Calibri", 12f, 0);
                _pdfPCell = new PdfPCell(new Phrase(edu.Title, _fontStyle));
                _pdfPCell.Colspan = 6;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfPTable.AddCell(_pdfPCell);

                //_fontStyle = FontFactory.GetFont("Calibri", 10f, 0);
                //_pdfPCell = new PdfPCell(new Phrase("3.59/4.00", _fontStyle));
                //_pdfPCell.Colspan = 2;
                //_pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                //_pdfPCell.Border = 0;
                //_pdfPCell.BackgroundColor = BaseColor.WHITE;
                //_pdfPCell.ExtraParagraphSpace = 0;
                //_pdfPTable.AddCell(_pdfPCell);
                NewLine();

            }


            _pdfPTable.CompleteRow();
            NewLine();
        }

        private void Experience()
        {
            NewLine();

            _fontStyle = FontFactory.GetFont("Calibri", 14f, 1);
            _fontStyle.SetColor(33, 97, 140);
            _pdfPCell = new PdfPCell(new Phrase("Experience", _fontStyle));
            _pdfPCell.Colspan = _totalColumn;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfPCell.Border = 0;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfPTable.AddCell(_pdfPCell);
            _pdfPTable.CompleteRow();

            foreach(var exp in _user.ExpVMList)
            {
                _fontStyle = FontFactory.GetFont("Calibri", 13f, 1);
                _fontStyle.SetColor(25, 111, 61);
                _pdfPCell = new PdfPCell(new Phrase(exp.Title, _fontStyle));
                _pdfPCell.Colspan = 4;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfPTable.AddCell(_pdfPCell);

                _fontStyle = FontFactory.GetFont("Calibri", 11f, 0);
                if (exp.CurrentWorkCheck)
                    _pdfPCell = new PdfPCell(new Phrase(exp.StartDate.ToString("dd MMM yyyy") + " - Current", _fontStyle));
                else
                    _pdfPCell = new PdfPCell(new Phrase(exp.StartDate.ToString("dd MMM yyyy") + " - " + exp.EndDate.ToString("dd MMM yyyy"), _fontStyle));

                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfPTable.AddCell(_pdfPCell);

                _pdfPTable.CompleteRow();

                _fontStyle = FontFactory.GetFont("Calibri", 12f, 0);
                _pdfPCell = new PdfPCell(new Phrase(exp.Description, _fontStyle));
                _pdfPCell.Colspan = _totalColumn;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfPTable.AddCell(_pdfPCell);
                NewLine();

            }

            _pdfPTable.CompleteRow();
            NewLine();

        }

        private void Certifications()
        {
            _fontStyle = FontFactory.GetFont("Calibri", 14f, 1);
            _fontStyle.SetColor(33, 97, 140);
            _pdfPCell = new PdfPCell(new Phrase("Certifications", _fontStyle));
            _pdfPCell.Colspan = _totalColumn;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfPCell.Border = 0;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfPTable.AddCell(_pdfPCell);
            _pdfPTable.CompleteRow();

            foreach(var u in _user.CertificationVMList)
            {
                _fontStyle = FontFactory.GetFont("Calibri", 13f, 1);
                _fontStyle.SetColor(25, 111, 61);
                _pdfPCell = new PdfPCell(new Phrase(u.Name, _fontStyle));
                _pdfPCell.Colspan = 4;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfPTable.AddCell(_pdfPCell);

                _fontStyle = FontFactory.GetFont("Calibri", 11f, 0);
                if (u.ExpirationDate==null)
                    _pdfPCell = new PdfPCell(new Phrase(u.CertificationDate.ToString("dd MMM yyyy") + " - No Expiry", _fontStyle));
                else
                    _pdfPCell = new PdfPCell(new Phrase(u.CertificationDate.ToString("dd MMM yyyy") + " - " + u.ExpirationDate.ToString("dd MMM yyyy"), _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfPTable.AddCell(_pdfPCell);

                _pdfPTable.CompleteRow();
                if (u.URL != null)
                {
                    _fontStyle = FontFactory.GetFont("Calibri", 12f, 0);
                    _pdfPCell = new PdfPCell(new Phrase("URL : " + u.URL, _fontStyle));
                    _pdfPCell.Colspan = _totalColumn;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.Border = 0;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfPCell.ExtraParagraphSpace = 0;
                    _pdfPTable.AddCell(_pdfPCell);

                }
                if (u.LiscenceNo != null)
                {
                    _fontStyle = FontFactory.GetFont("Calibri", 12f, 0);
                    _pdfPCell = new PdfPCell(new Phrase("\tLiscence no : " + u.LiscenceNo, _fontStyle));
                    _pdfPCell.Colspan = _totalColumn;
                    _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _pdfPCell.Border = 0;
                    _pdfPCell.BackgroundColor = BaseColor.WHITE;
                    _pdfPCell.ExtraParagraphSpace = 0;
                    _pdfPTable.AddCell(_pdfPCell);

                }
                _pdfPTable.CompleteRow();
                NewLine();
            }
            NewLine();
        }

        private void Achievements()
        {
            _fontStyle = FontFactory.GetFont("Calibri", 14f, 1);
            _fontStyle.SetColor(33, 97, 140);
            _pdfPCell = new PdfPCell(new Phrase("Achievements", _fontStyle));
            _pdfPCell.Colspan = _totalColumn;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfPCell.Border = 0;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfPTable.AddCell(_pdfPCell);
            _pdfPTable.CompleteRow();

            foreach (var u in _user.AchievVMList)
            {
                _fontStyle = FontFactory.GetFont("Calibri", 12f, 0);
                _pdfPCell = new PdfPCell(new Phrase("\t•    " + u.Description, _fontStyle));
                _pdfPCell.Colspan = _totalColumn;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfPTable.AddCell(_pdfPCell);

                _pdfPTable.CompleteRow();
            }
            NewLine();
        }

        private void Hobbies()
        {
            _fontStyle = FontFactory.GetFont("Calibri", 14f, 1);
            _fontStyle.SetColor(33, 97, 140);
            _pdfPCell = new PdfPCell(new Phrase("Hobbies", _fontStyle));
            _pdfPCell.Colspan = _totalColumn;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfPCell.Border = 0;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfPTable.AddCell(_pdfPCell);
            _pdfPTable.CompleteRow();

            foreach (var u in _user.HobbyVMList)
            {
                _fontStyle = FontFactory.GetFont("Calibri", 12f, 0);
                _pdfPCell = new PdfPCell(new Phrase("\t•    " + u.HobbyOrIntrest, _fontStyle));
                _pdfPCell.Colspan = _totalColumn;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfPTable.AddCell(_pdfPCell);

                _pdfPTable.CompleteRow();
            }
            NewLine();
        }

        private void LanguageSkills()
        {
            _fontStyle = FontFactory.GetFont("Calibri", 14f, 1);
            _fontStyle.SetColor(33, 97, 140);
            _pdfPCell = new PdfPCell(new Phrase("Languages Known", _fontStyle));
            _pdfPCell.Colspan = _totalColumn;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfPCell.Border = 0;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfPTable.AddCell(_pdfPCell);
            _pdfPTable.CompleteRow();

            foreach (var u in _user.LanguageSkillVMList)
            {
                _fontStyle = FontFactory.GetFont("Calibri", 12f, 0);
                _pdfPCell = new PdfPCell(new Phrase("\t•    " + u.SkillName, _fontStyle));
                _pdfPCell.Colspan = _totalColumn;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfPTable.AddCell(_pdfPCell);

                _pdfPTable.CompleteRow();
            }
            NewLine();
        }

        private void JournalP()
        {
            NewLine();

            _fontStyle = FontFactory.GetFont("Calibri", 14f, 1);
            _fontStyle.SetColor(33, 97, 140);
            _pdfPCell = new PdfPCell(new Phrase("Journal Publications", _fontStyle));
            _pdfPCell.Colspan = _totalColumn;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfPCell.Border = 0;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfPTable.AddCell(_pdfPCell);
            _pdfPTable.CompleteRow();

            foreach (var journal in _user.PublicationVMListJP)
            {
                _fontStyle = FontFactory.GetFont("Calibri", 13f, 1);
                _fontStyle.SetColor(25, 111, 61);
                _pdfPCell = new PdfPCell(new Phrase(journal.Title, _fontStyle));
                _pdfPCell.Colspan = 4;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfPTable.AddCell(_pdfPCell);

                _fontStyle = FontFactory.GetFont("Calibri", 11f, 0);
                _pdfPCell = new PdfPCell(new Phrase("Year Published: "+journal.PublishYear.ToString("yyyy"), _fontStyle));

                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfPTable.AddCell(_pdfPCell);

                _pdfPTable.CompleteRow();

                _fontStyle = FontFactory.GetFont("Calibri", 12f, 0);
                _pdfPCell = new PdfPCell(new Phrase(journal.Description, _fontStyle));
                _pdfPCell.Colspan = _totalColumn;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfPTable.AddCell(_pdfPCell);
                NewLine();

            }

            _pdfPTable.CompleteRow();
            NewLine();

        }

        private void ConferenceP()
        {
            NewLine();

            _fontStyle = FontFactory.GetFont("Calibri", 14f, 1);
            _fontStyle.SetColor(33, 97, 140);
            _pdfPCell = new PdfPCell(new Phrase("Conference Publications", _fontStyle));
            _pdfPCell.Colspan = _totalColumn;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfPCell.Border = 0;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfPTable.AddCell(_pdfPCell);
            _pdfPTable.CompleteRow();

            foreach (var conference in _user.PublicationVMListCP)
            {
                _fontStyle = FontFactory.GetFont("Calibri", 13f, 1);
                _fontStyle.SetColor(25, 111, 61);
                _pdfPCell = new PdfPCell(new Phrase(conference.Title, _fontStyle));
                _pdfPCell.Colspan = 4;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfPTable.AddCell(_pdfPCell);

                _fontStyle = FontFactory.GetFont("Calibri", 11f, 0);
                _pdfPCell = new PdfPCell(new Phrase("Year Published: " + conference.PublishYear.ToString("yyyy"), _fontStyle));

                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfPTable.AddCell(_pdfPCell);

                _pdfPTable.CompleteRow();

                _fontStyle = FontFactory.GetFont("Calibri", 12f, 0);
                _pdfPCell = new PdfPCell(new Phrase(conference.Description, _fontStyle));
                _pdfPCell.Colspan = _totalColumn;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfPTable.AddCell(_pdfPCell);
                NewLine();

            }

            _pdfPTable.CompleteRow();
            NewLine();

        }

        private void Skills()
        {
            _fontStyle = FontFactory.GetFont("Calibri", 14f, 1);
            _fontStyle.SetColor(33, 97, 140);
            _pdfPCell = new PdfPCell(new Phrase("Achievements", _fontStyle));
            _pdfPCell.Colspan = _totalColumn;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfPCell.Border = 0;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfPTable.AddCell(_pdfPCell);
            _pdfPTable.CompleteRow();

            for (int i = 1; i <= 5; i++)
            {
                _fontStyle = FontFactory.GetFont("Calibri", 12f, 0);
                _pdfPCell = new PdfPCell(new Phrase("\t•    Ex-Liaison Head, IEEE CUST Chapter" + i, _fontStyle));
                _pdfPCell.Colspan = _totalColumn;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                _pdfPCell.Border = 0;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                _pdfPTable.AddCell(_pdfPCell);

                _pdfPTable.CompleteRow();
            }
            NewLine();
        }
        private void HorizontalLine()
        {
            #region Table Line
            _fontStyle = FontFactory.GetFont("Tahoma", 0f, 1);
            _pdfPCell = new PdfPCell(new Phrase("Serial Number", _fontStyle));
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            _pdfPTable.AddCell(_pdfPCell);
            _pdfPTable.CompleteRow();
            #endregion
        }

        private void NewLine()
        {
            //New Line
            _fontStyle = FontFactory.GetFont("Arial", 10f, 0);
            _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
            _pdfPCell.Colspan = _totalColumn;
            _pdfPCell.Border = 0;
            _pdfPTable.AddCell(_pdfPCell);

            _pdfPTable.CompleteRow();
        }
    }
}
