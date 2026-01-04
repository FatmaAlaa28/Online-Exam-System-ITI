-- 1. Branches
INSERT INTO dbo.Branch (BranchName, Location, IsActive)
VALUES 
    ('Main Campus', 'Cairo Downtown', 1),
    ('Alexandria Branch', 'Smouha', 1),
    ('Giza Branch', 'Dokki', 1),
    ('New Cairo Branch', '5th Settlement', 1);

-- 2. Departments
INSERT INTO dbo.Department (DeptName, Description)
VALUES 
    ('Computer Science', 'Focus on software development, AI, and algorithms'),
    ('Information Systems', 'Business-oriented IT and systems analysis'),
    ('Cyber Security', 'Network security, ethical hacking, and cryptography'),
    ('Data Science & AI', 'Machine learning, big data, and analytics');

-- 3. Courses (some required for departments)
INSERT INTO dbo.Course (CourseName, CourseCode, Description, MaxDegree, CreditHours, IsActive)
VALUES 
    ('Database Systems', 'CS301', 'Relational databases, SQL, normalization', 100, 3, 1),
    ('Operating Systems', 'CS302', 'Process management, memory, file systems', 100, 3, 1),
    ('Data Structures & Algorithms', 'CS201', 'Arrays, trees, graphs, sorting, searching', 100, 3, 1),
    ('Introduction to Programming', 'CS101', 'Basics of programming using C++', 100, 3, 1),
    ('Web Development', 'IS301', 'HTML, CSS, JavaScript, backend frameworks', 100, 3, 1),
    ('Network Security', 'CS401', 'Firewalls, VPNs, intrusion detection', 100, 3, 1);

-- 4. DepartmentCourse (link courses to departments)
INSERT INTO dbo.DepartmentCourse (DeptID,CourseID, IsRequired)
VALUES 
    (1, 1, 1),  -- CS → Database Systems (required)
    (1, 2, 1),  -- CS → Operating Systems (required)
    (1, 3, 1),  -- CS → Data Structures (required)
    (1, 4, 1),  -- CS → Intro to Programming (required)
    (2, 5, 1),  -- IS → Web Development (required)
    (3, 6, 1);  -- Cyber Security → Network Security (required)

-- 5. Instructors
INSERT INTO dbo.Instructor (InstructorName, Email, Phone, HireDate, IsActive, BranchID)
VALUES 
    ('Dr. Ahmed Hassan', 'ahmed.hassan@iti.gov.eg', '0123456789', '2020-09-01', 1, 1),
    ('Eng. Sara Mohamed', 'sara.mohamed@iti.gov.eg', '0109876543', '2021-03-15', 1, 2),
    ('Prof. Khaled Ali', 'khaled.ali@iti.gov.eg', '0112233445', '2019-01-10', 1, 1),
    ('Ms. Nouran Tarek', 'nouran.tarek@iti.gov.eg', '0155566778', '2022-06-20', 1, 3);

-- 6. InstructorCourse (assign instructors to courses)
INSERT INTO dbo.InstructorCourse (InstructorID, CourseID, AssignmentDate)
VALUES 
    (1, 1, '2024-09-01'),  -- Dr. Ahmed → Database Systems
    (3, 2, '2024-09-01'),  -- Prof. Khaled → Operating Systems
    (1, 3, '2024-09-01'),  -- Dr. Ahmed → Data Structures
    (4, 5, '2024-09-01'),  -- Ms. Nouran → Web Development
    (2, 6, '2024-09-01');  -- Eng. Sara → Network Security

-- 7. Students
INSERT INTO dbo.Student (StudentName, Email, Phone, DeptID, BranchID, EnrollmentDate, IsActive)
VALUES 
    ('Omar Khaled', 'omar.khaled@student.edu', '0121112233', 1, 1, '2023-09-01', 1),
    ('Laila Mostafa', 'laila.mostafa@student.edu', '0103344556', 1, 1, '2023-09-01', 1),
    ('Youssef Ahmed', 'youssef.ahmed@student.edu', '0115566778', 2, 2, '2023-09-01', 1),
    ('Mariam Hassan', 'mariam.hassan@student.edu', '0156677889', 3, 3, '2024-09-01', 1),
    ('Ahmed Reda', 'ahmed.reda@student.edu', '0128899001', 1, 1, '2022-09-01', 1);

-- 8. StudentCourse (enroll students in courses)
INSERT INTO dbo.StudentCourse (StudentID, CourseID, EnrollmentDate, IsActive)
VALUES 
    --(1, 1, '2024-09-01', 1),  -- Omar → Database Systems
    --(1, 3, '2024-09-01', 1),  -- Omar → Data Structures
    (2, 1, '2024-09-01', 1),  -- Laila → Database Systems
    (3, 5, '2024-09-01', 1),  -- Youssef → Web Development
    (5, 2, '2024-09-01', 1);  -- Ahmed → Operating Systems

-- 9. Questions (for Database Systems - CourseID = 1)
INSERT INTO dbo.Question (QuestionText, QuestionType, Points, CourseID, DifficultyLevel, IsActive)
VALUES 
    ('What does ACID stand for in databases?', 'MCQ', 10, 1, 'Medium', 1),
    ('Which SQL keyword is used to combine rows from two or more tables?', 'MCQ', 5, 1, 'Easy', 1),
    ('Is a primary key always unique?', 'TF', 5, 1, 'Easy', 1);

-- 10. Choices (for the questions above)
INSERT INTO dbo.Choice (ChoiceText, QuestionID, IsCorrect)
VALUES 
    -- Q1: ACID
    ('Atomicity, Consistency, Isolation, Durability', 1, 1),
    ('Accuracy, Completeness, Integrity, Durability', 1, 0),
    ('Atomicity, Concurrency, Isolation, Durability', 1, 0),
    ('Atomicity, Consistency, Integration, Durability', 1, 0),

    -- Q2: JOIN
    ('JOIN', 2, 1),
    ('MERGE', 2, 0),
    ('UNION', 2, 0),
    ('COMBINE', 2, 0),

    -- Q3: True/False (TF)
    ('True', 3, 1),
    ('False', 3, 0);

-- 11. Exams (for Database Systems)
INSERT INTO dbo.Exam (CourseID, ExamName, ExamType, TotalDegree, TotalQuestions, DurationMinutes, PassingScore, ExamDate, CreatedBy, IsRandomized)
VALUES 
    (1, 'Midterm Exam - Database Systems', 'MCQ', 100, 10, 90, 60, '2025-01-15', 1, 1);

-- 12. ExamQuestion (assign questions to exam)
INSERT INTO dbo.ExamQuestion (ExamID, QuestionID, QuestionOrder)
VALUES 
    (1, 1, 1),
    (1, 2, 2),
    (1, 3, 3);

-- 13. ExamAssignment (assign exam to students)
INSERT INTO dbo.ExamAssignment (ExamID, StudentID, AssignedDate, DueDate, IsActive)
VALUES 
    --(1, 1, '2025-01-01', '2025-01-15', 1),  -- Omar
    (1, 2, '2025-01-01', '2025-01-15', 1),  -- Laila
    (1, 5, '2025-01-01', '2025-01-15', 1);  -- Ahmed



---------------------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.StartExam
    @StudentID INT,
    @ExamID INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF NOT EXISTS (
            SELECT 1 FROM dbo.ExamAssignment
            WHERE StudentID = @StudentID AND ExamID = @ExamID AND IsActive = 1
        )
            THROW 50001, 'This exam is not assigned to the student.', 1;

        IF NOT EXISTS (
            SELECT 1 FROM dbo.StudentExam
            WHERE StudentID = @StudentID AND ExamID = @ExamID
        )
        BEGIN
            INSERT INTO dbo.StudentExam (StudentID, ExamID, StartTime)
            VALUES (@StudentID, @ExamID, GETDATE());
        END

        SELECT 'Exam started successfully.' AS Message;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO


---------------------------------------------

CREATE OR ALTER PROCEDURE dbo.SubmitExamAnswers
    @StudentID INT,
    @ExamID INT,
    @Q1 INT = NULL,
    @Q2 INT = NULL,
    @Q3 INT = NULL,
    @Q4 INT = NULL,
    @Q5 INT = NULL,
    @Q6 INT = NULL,
    @Q7 INT = NULL,
    @Q8 INT = NULL,
    @Q9 INT = NULL,
    @Q10 INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (SELECT 1 FROM dbo.StudentExam WHERE StudentID = @StudentID AND ExamID = @ExamID)
            THROW 50001, 'The student has not yet started the exam.', 1;

        IF EXISTS (SELECT 1 FROM dbo.StudentExam WHERE StudentID = @StudentID AND ExamID = @ExamID AND SubmittedDate IS NOT NULL)
            THROW 50002, 'The student already submitted the exam.', 1;

        INSERT INTO dbo.StudentAnswer (StudentID, ExamID, QuestionID, SelectedChoiceID, QuestionPoints, EarnedPoints, IsCorrect, AnsweredDate)
        SELECT
            @StudentID,
            @ExamID,
            eq.QuestionID,
            A.SelectedChoiceID,
            q.Points,
            0,
            NULL,
            GETDATE()
        FROM (
            SELECT 1 AS QOrder, @Q1 AS SelectedChoiceID UNION ALL
            SELECT 2, @Q2 UNION ALL
            SELECT 3, @Q3 UNION ALL
            SELECT 4, @Q4 UNION ALL
            SELECT 5, @Q5 UNION ALL
            SELECT 6, @Q6 UNION ALL
            SELECT 7, @Q7 UNION ALL
            SELECT 8, @Q8 UNION ALL
            SELECT 9, @Q9 UNION ALL
            SELECT 10, @Q10
        ) A
        INNER JOIN dbo.ExamQuestion eq ON eq.ExamID = @ExamID AND eq.QuestionOrder = A.QOrder
        INNER JOIN dbo.Question q ON eq.QuestionID = q.QuestionID;

        UPDATE dbo.StudentExam
        SET SubmittedDate = GETDATE(),
            EndTime = GETDATE(),
            IsCorrected = 0
        WHERE StudentID = @StudentID AND ExamID = @ExamID;

        COMMIT TRANSACTION;

        SELECT 'Exam submitted successfully. Awaiting grading.' AS Message;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO


------------------------------------------------


CREATE OR ALTER PROCEDURE dbo.ExamCorrection
    @ExamID INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Scores TABLE (
        StudentID   INT,
        points INT,
        TotalPoints INT
    );

    declare @PassingScore int
    Select @PassingScore =  E.PassingScore from Exam E where E.ExamID =  @ExamID;
    
    -- Calculate scores (handle unanswered questions correctly)
    INSERT INTO @Scores (StudentID,points, TotalPoints)
    SELECT 
        SA.StudentID,
        SUM(CASE 
                WHEN C.IsCorrect = 1 THEN Q.Points 
                ELSE 0 
            END) ASpoints,
        SUM(Q.Points) AS TotalPoints
    FROM dbo.StudentAnswer SA
    INNER JOIN dbo.Question Q ON SA.QuestionID = Q.QuestionID
    LEFT JOIN dbo.Choice C 
        ON C.ChoiceID = SA.SelectedChoiceID 
       AND C.QuestionID = Q.QuestionID
    WHERE SA.ExamID = @ExamID
    GROUP BY SA.StudentID;

    -- MERGE statement with correct syntax
    MERGE dbo.ExamGrade AS target
    USING @Scores AS source
    ON target.StudentID = source.StudentID 
       AND target.ExamID = @ExamID
    WHEN MATCHED THEN
        UPDATE SET
            TotalScore = source.Points,
            MaxScore   = source.TotalPoints,
            Percentage = CASE 
                            WHEN source.TotalPoints > 0 
                            THEN ROUND(CAST(source.Points AS DECIMAL(10,2)) / source.TotalPoints * 100, 2)
                            ELSE 0 
                         END,
            Grade = CASE 
                        WHEN source.TotalPoints = 0 THEN 'N/A'
                        WHEN CAST(source.Points AS DECIMAL(10,2)) / source.TotalPoints >= 0.85 THEN 'A'
                        WHEN CAST(source.Points AS DECIMAL(10,2)) / source.TotalPoints >= 0.75 THEN 'B'
                        WHEN CAST(source.Points AS DECIMAL(10,2)) / source.TotalPoints >= 0.65 THEN 'C'
                        WHEN CAST(source.Points AS DECIMAL(10,2)) / source.TotalPoints >= 0.50 THEN 'D'
                        ELSE 'F'
                    END,
            Status = CASE 
                        WHEN source.TotalPoints = 0 THEN 'Pending'
                        WHEN source.Points >= @PassingScore THEN 'Pass'
                        ELSE 'Fail'
                     END,
            GradeDate = GETDATE()
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (StudentID, ExamID, TotalScore, MaxScore, Percentage, Grade, Status, GradeDate)
        VALUES (
            source.StudentID,
            @ExamID,
            source.Points,
            source.TotalPoints,
            CASE 
                WHEN source.TotalPoints > 0 
                THEN ROUND(CAST(source.Points AS DECIMAL(10,2)) / source.TotalPoints * 100, 2)
                ELSE 0 
            END,
            CASE 
                WHEN source.TotalPoints = 0 THEN 'N/A'
                WHEN CAST(source.Points AS DECIMAL(10,2)) / source.TotalPoints >= 0.85 THEN 'A'
                WHEN CAST(source.Points AS DECIMAL(10,2)) / source.TotalPoints >= 0.75 THEN 'B'
                WHEN CAST(source.Points AS DECIMAL(10,2)) / source.TotalPoints >= 0.65 THEN 'C'
                WHEN CAST(source.Points AS DECIMAL(10,2)) / source.TotalPoints >= 0.50 THEN 'D'
                ELSE 'F'
            END,
            CASE 
                WHEN source.TotalPoints = 0 THEN 'Pending'
                WHEN source.Points >= @PassingScore THEN 'Pass'
                ELSE 'Fail'
            END,
            GETDATE()
        );
   

    -- Mark exam as corrected (optional)
    UPDATE dbo.StudentExam
    SET IsCorrected = 1,
        SubmittedDate = GETDATE()
    WHERE ExamID = @ExamID;

END;
GO


Exec ExamCorrection 1

----------------

CREATE OR ALTER PROCEDURE SP_GenerateExam
    @CourseID INT,
    @ExamName VARCHAR(100),
    @NumMCQ INT,
    @NumTF INT,
    @InstructorID INT,
    @ExamDate DATETIME = NULL,
    @DurationMinutes INT = 60,
    @PassingPercentage DECIMAL(5,2) = 60.0, -- Default 60%
    @NewExamID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        /* ===============================
           1️⃣ Enhanced Validation
        =============================== */

        -- Course exists and is active
        IF NOT EXISTS (
            SELECT 1 FROM Course 
            WHERE CourseID = @CourseID AND IsActive = 1
        )
            THROW 50001, 'Course does not exist or is inactive.', 1;

        -- Instructor exists and is active
        IF NOT EXISTS (
            SELECT 1 FROM Instructor 
            WHERE InstructorID = @InstructorID AND IsActive = 1
        )
            THROW 50002, 'Instructor does not exist or is inactive.', 1;

        -- Instructor teaches this course
        IF NOT EXISTS (
            SELECT 1 FROM InstructorCourse 
            WHERE InstructorID = @InstructorID AND CourseID = @CourseID
        )
            THROW 50003, 'Instructor does not teach this course.', 1;

        -- Exam name is not empty
        IF @ExamName IS NULL OR LTRIM(RTRIM(@ExamName)) = ''
            THROW 50004, 'Exam name cannot be empty.', 1;

        -- Question counts validation
        IF @NumMCQ < 0 OR @NumTF < 0
            THROW 50005, 'Number of questions cannot be negative.', 1;

        IF @NumMCQ = 0 AND @NumTF = 0
            THROW 50006, 'Exam must have at least one question.', 1;

        -- Duration validation
        IF @DurationMinutes <= 0
            THROW 50007, 'Exam duration must be greater than 0.', 1;

        -- Passing percentage validation
        IF @PassingPercentage < 0 OR @PassingPercentage > 100
            THROW 50008, 'Passing percentage must be between 0 and 100.', 1;

        -- Exam date validation
        IF @ExamDate IS NULL
            SET @ExamDate = GETDATE();
        
        IF @ExamDate < CAST(GETDATE() AS DATE)
            THROW 50009, 'Exam date cannot be in the past.', 1;

        /* ===============================
           2️⃣ Check Question Availability by Difficulty
        =============================== */

        DECLARE @AvailableMCQ INT, @AvailableTF INT;
        DECLARE @AvailableMCQ_Easy INT, @AvailableMCQ_Medium INT, @AvailableMCQ_Hard INT;
        DECLARE @AvailableTF_Easy INT, @AvailableTF_Medium INT, @AvailableTF_Hard INT;

        -- Total available questions
        SELECT @AvailableMCQ = COUNT(*)
        FROM Question
        WHERE CourseID = @CourseID
          AND QuestionType = 'MCQ'
          AND IsActive = 1;

        SELECT @AvailableTF = COUNT(*)
        FROM Question
        WHERE CourseID = @CourseID
          AND QuestionType = 'TF'
          AND IsActive = 1;

        IF @AvailableMCQ < @NumMCQ
        BEGIN
            DECLARE @MCQMsg NVARCHAR(200) = 
                'Not enough MCQ questions. Available: ' + CAST(@AvailableMCQ AS VARCHAR) + 
                ', Required: ' + CAST(@NumMCQ AS VARCHAR);
            THROW 50010, @MCQMsg, 1;
        END

        IF @AvailableTF < @NumTF
        BEGIN
            DECLARE @TFMsg NVARCHAR(200) = 
                'Not enough True/False questions. Available: ' + CAST(@AvailableTF AS VARCHAR) + 
                ', Required: ' + CAST(@NumTF AS VARCHAR);
            THROW 50011, @TFMsg, 1;
        END

        -- Check difficulty distribution
        SELECT 
            @AvailableMCQ_Easy = COUNT(CASE WHEN DifficultyLevel = 'Easy' THEN 1 END),
            @AvailableMCQ_Medium = COUNT(CASE WHEN DifficultyLevel = 'Medium' THEN 1 END),
            @AvailableMCQ_Hard = COUNT(CASE WHEN DifficultyLevel = 'Hard' THEN 1 END)
        FROM Question
        WHERE CourseID = @CourseID
          AND QuestionType = 'MCQ'
          AND IsActive = 1;

        SELECT 
            @AvailableTF_Easy = COUNT(CASE WHEN DifficultyLevel = 'Easy' THEN 1 END),
            @AvailableTF_Medium = COUNT(CASE WHEN DifficultyLevel = 'Medium' THEN 1 END),
            @AvailableTF_Hard = COUNT(CASE WHEN DifficultyLevel = 'Hard' THEN 1 END)
        FROM Question
        WHERE CourseID = @CourseID
          AND QuestionType = 'TF'
          AND IsActive = 1;

        /* ===============================
           3️⃣ Smart Question Selection (Balanced Difficulty)
        =============================== */

        DECLARE @SelectedQuestions TABLE (
            QuestionID INT PRIMARY KEY,
            QuestionOrder INT,
            QuestionType VARCHAR(10),
            Points INT
        );

        -- Calculate optimal distribution (40% Easy, 40% Medium, 20% Hard)
        DECLARE @NumMCQ_Easy INT = CEILING(@NumMCQ * 0.4);
        DECLARE @NumMCQ_Medium INT = CEILING(@NumMCQ * 0.4);
        DECLARE @NumMCQ_Hard INT = @NumMCQ - @NumMCQ_Easy - @NumMCQ_Medium;

        DECLARE @NumTF_Easy INT = CEILING(@NumTF * 0.4);
        DECLARE @NumTF_Medium INT = CEILING(@NumTF * 0.4);
        DECLARE @NumTF_Hard INT = @NumTF - @NumTF_Easy - @NumTF_Medium;

        -- Adjust if not enough questions in a difficulty level
        IF @NumMCQ_Easy > @AvailableMCQ_Easy
        BEGIN
            SET @NumMCQ_Medium = @NumMCQ_Medium + (@NumMCQ_Easy - @AvailableMCQ_Easy);
            SET @NumMCQ_Easy = @AvailableMCQ_Easy;
        END

        IF @NumMCQ_Medium > @AvailableMCQ_Medium
        BEGIN
            SET @NumMCQ_Easy = @NumMCQ_Easy + (@NumMCQ_Medium - @AvailableMCQ_Medium);
            SET @NumMCQ_Medium = @AvailableMCQ_Medium;
        END

        -- Select MCQ Questions by Difficulty
        INSERT INTO @SelectedQuestions (QuestionID, QuestionOrder, QuestionType, Points)
        SELECT TOP (@NumMCQ_Easy)
            QuestionID,
            ROW_NUMBER() OVER (ORDER BY NEWID()),
            QuestionType,
            Points
        FROM Question
        WHERE CourseID = @CourseID
          AND QuestionType = 'MCQ'
          AND DifficultyLevel = 'Easy'
          AND IsActive = 1
        ORDER BY NEWID();

        INSERT INTO @SelectedQuestions (QuestionID, QuestionOrder, QuestionType, Points)
        SELECT TOP (@NumMCQ_Medium)
            QuestionID,
            @NumMCQ_Easy + ROW_NUMBER() OVER (ORDER BY NEWID()),
            QuestionType,
            Points
        FROM Question
        WHERE CourseID = @CourseID
          AND QuestionType = 'MCQ'
          AND DifficultyLevel = 'Medium'
          AND IsActive = 1
          AND QuestionID NOT IN (SELECT QuestionID FROM @SelectedQuestions)
        ORDER BY NEWID();

        INSERT INTO @SelectedQuestions (QuestionID, QuestionOrder, QuestionType, Points)
        SELECT TOP (@NumMCQ_Hard)
            QuestionID,
            @NumMCQ_Easy + @NumMCQ_Medium + ROW_NUMBER() OVER (ORDER BY NEWID()),
            QuestionType,
            Points
        FROM Question
        WHERE CourseID = @CourseID
          AND QuestionType = 'MCQ'
          AND DifficultyLevel = 'Hard'
          AND IsActive = 1
          AND QuestionID NOT IN (SELECT QuestionID FROM @SelectedQuestions)
        ORDER BY NEWID();

        -- Select True/False Questions
        INSERT INTO @SelectedQuestions (QuestionID, QuestionOrder, QuestionType, Points)
        SELECT TOP (@NumTF)
            QuestionID,
            @NumMCQ + ROW_NUMBER() OVER (ORDER BY NEWID()),
            QuestionType,
            Points
        FROM Question
        WHERE CourseID = @CourseID
          AND QuestionType = 'TF'
          AND IsActive = 1
          AND QuestionID NOT IN (SELECT QuestionID FROM @SelectedQuestions)
        ORDER BY NEWID();

        /* ===============================
           4️⃣ Calculate Exam Metrics
        =============================== */

        DECLARE @TotalDegree INT;
        DECLARE @TotalQuestions INT;
        DECLARE @PassingScore INT;

        SELECT 
            @TotalDegree = SUM(Points),
            @TotalQuestions = COUNT(*)
        FROM @SelectedQuestions;

        IF @TotalDegree IS NULL OR @TotalDegree <= 0
            THROW 50012, 'Failed to calculate total exam degree.', 1;

        IF @TotalQuestions <> (@NumMCQ + @NumTF)
            THROW 50013, 'Question selection mismatch.', 1;

        SET @PassingScore = CEILING(@TotalDegree * (@PassingPercentage / 100.0));

        /* ===============================
           5️⃣ Determine Exam Type
        =============================== */

        DECLARE @ExamType VARCHAR(20);

        IF @NumMCQ > 0 AND @NumTF > 0
            SET @ExamType = 'Mixed';
        ELSE IF @NumMCQ > 0
            SET @ExamType = 'MCQ';
        ELSE
            SET @ExamType = 'TF';

        /* ===============================
           6️⃣ Insert Exam Record
        =============================== */

        INSERT INTO Exam (
            CourseID, 
            ExamName,
            ExamType, 
            TotalDegree, 
            TotalQuestions,
            DurationMinutes, 
            PassingScore,
            ExamDate, 
            CreatedBy,
            IsRandomized
        )
        VALUES (
            @CourseID, 
            @ExamName,
            @ExamType, 
            @TotalDegree, 
            @TotalQuestions,
            @DurationMinutes, 
            @PassingScore,
            @ExamDate, 
            @InstructorID,
            1
        );

        SET @NewExamID = SCOPE_IDENTITY();

        /* ===============================
           7️⃣ Insert Exam Questions
        =============================== */

        INSERT INTO ExamQuestion (ExamID, QuestionID, QuestionOrder)
        SELECT
            @NewExamID,
            QuestionID,
            QuestionOrder
        FROM @SelectedQuestions
        ORDER BY QuestionOrder;

        COMMIT TRANSACTION;

        /* ===============================
           8️⃣ Return Exam Details
        =============================== */

        SELECT
            E.ExamID,
            E.ExamName,
            E.CourseID,
            C.CourseName,
            E.ExamType,
            E.TotalDegree,
            E.TotalQuestions,
            E.PassingScore,
            E.DurationMinutes,
            E.ExamDate,
            I.InstructorName AS CreatedBy,
            (SELECT COUNT(*) FROM ExamQuestion WHERE ExamID = E.ExamID) AS QuestionsCount,
            (SELECT COUNT(DISTINCT QuestionType) FROM @SelectedQuestions) AS QuestionTypesCount,
            'Exam generated successfully!' AS Message
        FROM Exam E
        INNER JOIN Course C ON E.CourseID = C.CourseID
        INNER JOIN Instructor I ON E.CreatedBy = I.InstructorID
        WHERE E.ExamID = @NewExamID;

        -- Return question distribution
        SELECT 
            QuestionType,
            COUNT(*) AS QuestionCount,
            SUM(Points) AS TotalPoints,
            MIN(Points) AS MinPoints,
            MAX(Points) AS MaxPoints,
            AVG(Points) AS AvgPoints
        FROM @SelectedQuestions
        GROUP BY QuestionType;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        DECLARE @ErrorLine INT = ERROR_LINE();
        DECLARE @ErrorNumber INT = ERROR_NUMBER();

        -- Enhanced error reporting
        DECLARE @FullError NVARCHAR(MAX) = 
            'Error ' + CAST(@ErrorNumber AS VARCHAR) + 
            ' at Line ' + CAST(@ErrorLine AS VARCHAR) + 
            ': ' + @ErrorMessage;

        RAISERROR (@FullError, @ErrorSeverity, @ErrorState);
        
        RETURN -1;
    END CATCH
END;
GO



--------------------------------------------- Report --------------------------------------------------------------

---•	Report that returns the students information according to Department No parameter.

CREATE PROCEDURE sp_GetStudentsByDepartment
    @DeptID INT
AS
BEGIN
    SELECT 
     *
    FROM Student
    WHERE DeptID = @DeptID;
END;

EXEC sp_GetStudentsByDepartment @DeptID = 3;

----•	Report that takes the student ID and returns the grades of the student in all courses. 
CREATE PROCEDURE sp_GetStudentCourseGrades
    @StudentID INT
AS
BEGIN
    SELECT 
        c.CourseName,
        cg.TotalScore,
        cg.MaxScore,
        cg.Percentage,
        cg.LetterGrade,
        cg.Status,
        cg.GradeDate
    FROM CourseGrade cg
    JOIN Course c 
        ON cg.CourseID = c.CourseID
    WHERE cg.StudentID = @StudentID;
END;

exec sp_GetStudentCourseGrades 6;

----- •	Report that takes course ID and returns its topics  
CREATE PROC SP_GETTOPICESFROMCOURSES 
@Course_id Int 
AS 
begin 
select TopicName from Topic where CourseID = @Course_id
end;

exec SP_GETTOPICESFROMCOURSES 3;


---•	Report that takes the instructor ID and returns the name of the courses that he teaches and the number of student per course.
CREATE PROCEDURE sp_GetInstructorCoursesWithStudentCount
    @InstructorID INT
AS
BEGIN
    SELECT 
        c.CourseName,
        COUNT(sc.StudentID) AS NumberOfStudents
    FROM InstructorCourse ic
    JOIN Course c ON ic.CourseID = c.CourseID
    LEFT JOIN StudentCourse sc ON c.CourseID = sc.CourseID
    WHERE ic.InstructorID = @InstructorID
    GROUP BY c.CourseName;
END;


exec sp_GetInstructorCoursesWithStudentCount 5;

---	Report that takes exam number and returns the Questions in it and choices [freeform report]
CREATE PROCEDURE sp_examQuestionsByExam
    @ExamID INT
AS
BEGIN
    SELECT 
        eq.QuestionOrder,
        q.QuestionText,
        STRING_AGG(c.ChoiceText, CHAR(13)+CHAR(10)) AS Choices
    FROM ExamQuestion eq
    JOIN Question q ON eq.QuestionID = q.QuestionID
    JOIN Choice c ON c.QuestionID = q.QuestionID
    WHERE eq.ExamID = @ExamID
    GROUP BY eq.QuestionOrder, q.QuestionText
    ORDER BY eq.QuestionOrder;
END;

exec sp_examQuestionsByExam  3;

---•	Report that takes exam number and the student ID then returns the Questions in this exam with the student answers. 

INSERT INTO StudentAnswer
(StudentID, ExamID, QuestionID, SelectedChoiceID, QuestionPoints, EarnedPoints, IsCorrect, AnsweredDate)
VALUES
-- طالب 3، امتحان 3
(3, 3, 1, 1, 10, 10, 1, GETDATE()),
(3, 3, 2, 2, 10, 0, 0, GETDATE()),
(3, 3, 3, 5, 10, 10, 1, GETDATE()),
(3, 3, 4, 7, 5, 5, 1, GETDATE()),
(3, 3, 5, 9, 5, 5, 1, GETDATE()),

-- طالب 4، امتحان 3
(4, 3, 1, 1, 10, 10, 1, GETDATE()),
(4, 3, 2, 2, 10, 0, 0, GETDATE()),
(4, 3, 3, 5, 10, 10, 1, GETDATE()),
(4, 3, 4, 7, 5, 5, 1, GETDATE()),
(4, 3, 5, 9, 5, 5, 1, GETDATE()),

-- طالب 3، امتحان 4
(3, 4, 6, 16, 10, 10, 1, GETDATE()),
(3, 4, 7, 17, 5, 5, 1, GETDATE()),
(3, 4, 8, 18, 5, 5, 1, GETDATE()),
(3, 4, 9, 19, 10, 10, 1, GETDATE()),
(3, 4, 10, 20, 5, 5, 1, GETDATE());



CREATE PROCEDURE sp_ExamStudentAnswersFormatted
    @ExamID INT,
    @StudentID INT
AS
BEGIN
    SELECT 
        eq.QuestionOrder AS QNumber,
        q.QuestionText,
        STRING_AGG(
            CASE 
                WHEN c.IsCorrect = 1 THEN '✔ ' + c.ChoiceText + '        ← Correct Answer'
                WHEN c.ChoiceID = sa.SelectedChoiceID THEN '← ' + c.ChoiceText + '            ← Student Answer'
                ELSE '  ' + c.ChoiceText
            END, CHAR(13) + CHAR(10)
        ) AS Choices
    FROM ExamQuestion eq
    JOIN Question q ON eq.QuestionID = q.QuestionID
    JOIN Choice c ON c.QuestionID = q.QuestionID
    LEFT JOIN StudentAnswer sa 
           ON sa.QuestionID = q.QuestionID 
          AND sa.ExamID = eq.ExamID 
          AND sa.StudentID = @StudentID
    WHERE eq.ExamID = @ExamID
    GROUP BY eq.QuestionOrder, q.QuestionText
    ORDER BY eq.QuestionOrder;
END;

EXEC sp_ExamStudentAnswersFormatted 
    @ExamID = 3, 
    @StudentID = 3; 

    -- =================================================================================
-- Stored Procedures for CRUD Operations (40 SPs)
-- =================================================================================

-- ---------------------------------------------------------------------------------
-- 1. BRANCH Table SPs
-- ---------------------------------------------------------------------------------

-- Select All/By ID
CREATE OR ALTER PROCEDURE dbo.sp_Branch_Select
    @BranchID INT = NULL
AS
BEGIN
    SELECT BranchID, BranchName, Location, IsActive
    FROM dbo.Branch
    WHERE (@BranchID IS NULL OR BranchID = @BranchID);
END
GO

-- Insert
CREATE OR ALTER PROCEDURE dbo.sp_Branch_Insert
    @BranchName NVARCHAR(100),
    @Location NVARCHAR(255),
    @IsActive BIT,
    @NewBranchID INT OUTPUT
AS
BEGIN
    INSERT INTO dbo.Branch (BranchName, Location, IsActive)
    VALUES (@BranchName, @Location, @IsActive);

    SET @NewBranchID = SCOPE_IDENTITY();
END
GO

-- Update
CREATE OR ALTER PROCEDURE dbo.sp_Branch_Update
    @BranchID INT,
    @BranchName NVARCHAR(100),
    @Location NVARCHAR(255),
    @IsActive BIT
AS
BEGIN
    UPDATE dbo.Branch
    SET
        BranchName = @BranchName,
        Location = @Location,
        IsActive = @IsActive
    WHERE BranchID = @BranchID;
END
GO

-- Delete
CREATE OR ALTER PROCEDURE dbo.sp_Branch_Delete
    @BranchID INT
AS
BEGIN
    DELETE FROM dbo.Branch
    WHERE BranchID = @BranchID;
END
GO

-- ---------------------------------------------------------------------------------
-- 2. DEPARTMENT Table SPs
-- ---------------------------------------------------------------------------------
ALTER TABLE department
ADD IsActive bit NOT NULL DEFAULT 1;

-- Select All/By ID
CREATE OR ALTER PROCEDURE dbo.sp_Department_Select
    @DeptID INT = NULL
AS
BEGIN
    SELECT DeptID, DeptName, Description, IsActive
    FROM dbo.Department
    WHERE (@DeptID IS NULL OR DeptID = @DeptID);
END
GO

-- Insert
CREATE OR ALTER PROCEDURE dbo.sp_Department_Insert
    @DeptName NVARCHAR(100),
    @Description NVARCHAR(MAX),
    @IsActive BIT,
    @NewDeptID INT OUTPUT
AS
BEGIN
    INSERT INTO dbo.Department (DeptName, Description, IsActive)
    VALUES (@DeptName, @Description, @IsActive);

    SET @NewDeptID = SCOPE_IDENTITY();
END
GO

-- Update
CREATE OR ALTER PROCEDURE dbo.sp_Department_Update
    @DeptID INT,
    @DeptName NVARCHAR(100),
    @Description NVARCHAR(MAX),
    @IsActive BIT
AS
BEGIN
    UPDATE dbo.Department
    SET
        DeptName = @DeptName,
        Description = @Description,
        IsActive = @IsActive
    WHERE DeptID = @DeptID;
END
GO

-- Delete
CREATE OR ALTER PROCEDURE dbo.sp_Department_Delete
    @DeptID INT
AS
BEGIN
    DELETE FROM dbo.Department
    WHERE DeptID = @DeptID;
END
GO

-- ---------------------------------------------------------------------------------
-- 3. INSTRUCTOR Table SPs
-- ---------------------------------------------------------------------------------

-- Select All/By ID
CREATE OR ALTER PROCEDURE dbo.sp_Instructor_Select
    @InstructorID INT = NULL
AS
BEGIN
    SELECT InstructorID, InstructorName, Email, Phone, HireDate, BranchID, BranchID, IsActive
    FROM dbo.Instructor
    WHERE (@InstructorID IS NULL OR InstructorID = @InstructorID);
END
GO

-- Insert
CREATE OR ALTER PROCEDURE dbo.sp_Instructor_Insert
    @InstructorName NVARCHAR(100),
    @Email NVARCHAR(100),
    @Phone NVARCHAR(20),
    @HireDate DATE,
    @BranchID INT,
    @IsActive BIT,
    @NewInstructorID INT OUTPUT
AS
BEGIN
    INSERT INTO dbo.Instructor (InstructorName, Email, Phone, HireDate, BranchID,  IsActive)
    VALUES (@InstructorName, @Email, @Phone, @HireDate, @BranchID,  @IsActive);

    SET @NewInstructorID = SCOPE_IDENTITY();
END
GO

-- Update
CREATE OR ALTER PROCEDURE dbo.sp_Instructor_Update
    @InstructorID INT,
    @InstructorName NVARCHAR(100),
    @Email NVARCHAR(100),
    @Phone NVARCHAR(20),
    @HireDate DATE,
    @BranchID INT,
    @IsActive BIT
AS
BEGIN
    UPDATE dbo.Instructor
    SET
        InstructorName = @InstructorName,
        Email = @Email,
        Phone = @Phone,
        HireDate = @HireDate,
        BranchID = @BranchID,
        IsActive = @IsActive
    WHERE InstructorID = @InstructorID;
END
GO

-- Delete
CREATE OR ALTER PROCEDURE dbo.sp_Instructor_Delete
    @InstructorID INT
AS
BEGIN
    DELETE FROM dbo.Instructor
    WHERE InstructorID = @InstructorID;
END
GO

-- ---------------------------------------------------------------------------------
-- 4. STUDENT Table SPs
-- ---------------------------------------------------------------------------------

-- Select All/By ID
CREATE OR ALTER PROCEDURE dbo.sp_Student_Select
    @StudentID INT = NULL
AS
BEGIN
    SELECT StudentID, StudentName, Email, Phone, DeptID, BranchID, EnrollmentDate, IsActive
    FROM dbo.Student
    WHERE (@StudentID IS NULL OR StudentID = @StudentID);
END
GO

-- Insert
CREATE OR ALTER PROCEDURE dbo.sp_Student_Insert
    @StudentName NVARCHAR(100),
    @Email NVARCHAR(100),
    @Phone NVARCHAR(20),
    @DeptID INT,
    @BranchID INT,
    @EnrollmentDate DATE,
    @IsActive BIT,
    @NewStudentID INT OUTPUT
AS
BEGIN
    INSERT INTO dbo.Student (StudentName, Email, Phone, DeptID, BranchID, EnrollmentDate, IsActive)
    VALUES (@StudentName, @Email, @Phone, @DeptID, @BranchID, @EnrollmentDate, @IsActive);

    SET @NewStudentID = SCOPE_IDENTITY();
END
GO

-- Update
CREATE OR ALTER PROCEDURE dbo.sp_Student_Update
    @StudentID INT,
    @StudentName NVARCHAR(100),
    @Email NVARCHAR(100),
    @Phone NVARCHAR(20),
    @DeptID INT,
    @BranchID INT,
    @EnrollmentDate DATE,
    @IsActive BIT
AS
BEGIN
    UPDATE dbo.Student
    SET
        StudentName = @StudentName,
        Email = @Email,
        Phone = @Phone,
        DeptID = @DeptID,
        BranchID = @BranchID,
        EnrollmentDate = @EnrollmentDate,
        IsActive = @IsActive
    WHERE StudentID = @StudentID;
END
GO

-- Delete
CREATE OR ALTER PROCEDURE dbo.sp_Student_Delete
    @StudentID INT
AS
BEGIN
    DELETE FROM dbo.Student
    WHERE StudentID = @StudentID;
END
GO

-- ---------------------------------------------------------------------------------
-- 5. COURSE Table SPs
-- ---------------------------------------------------------------------------------

-- Select All/By ID
CREATE OR ALTER PROCEDURE dbo.sp_Course_Select
    @CourseID INT = NULL
AS
BEGIN
    SELECT CourseID, CourseName, CourseCode, Description, MaxDegree, CreditHours, IsActive
    FROM dbo.Course
    WHERE (@CourseID IS NULL OR CourseID = @CourseID);
END
GO

-- Insert
CREATE OR ALTER PROCEDURE dbo.sp_Course_Insert
    @CourseName NVARCHAR(100),
    @CourseCode NVARCHAR(10),
    @Description NVARCHAR(MAX),
    @MaxDegree INT,
    @CreditHours INT,
    @IsActive BIT,
    @NewCourseID INT OUTPUT
AS
BEGIN
    INSERT INTO dbo.Course (CourseName, CourseCode, Description, MaxDegree, CreditHours, IsActive)
    VALUES (@CourseName, @CourseCode, @Description, @MaxDegree, @CreditHours, @IsActive);

    SET @NewCourseID = SCOPE_IDENTITY();
END
GO

-- Update
CREATE OR ALTER PROCEDURE dbo.sp_Course_Update
    @CourseID INT,
    @CourseName NVARCHAR(100),
    @CourseCode NVARCHAR(10),
    @Description NVARCHAR(MAX),
    @MaxDegree INT,
    @CreditHours INT,
    @IsActive BIT
AS
BEGIN
    UPDATE dbo.Course
    SET
        CourseName = @CourseName,
        CourseCode = @CourseCode,
        Description = @Description,
        MaxDegree = @MaxDegree,
        CreditHours = @CreditHours,
        IsActive = @IsActive
    WHERE CourseID = @CourseID;
END
GO

-- Delete
CREATE OR ALTER PROCEDURE dbo.sp_Course_Delete
    @CourseID INT
AS
BEGIN
    DELETE FROM dbo.Course
    WHERE CourseID = @CourseID;
END
GO

-- ---------------------------------------------------------------------------------
-- 6. TOPIC Table SPs
-- ---------------------------------------------------------------------------------

-- Select All/By ID
CREATE OR ALTER PROCEDURE dbo.sp_Topic_Select
    @TopicID INT = NULL
AS
BEGIN
    SELECT TopicID, TopicName, CourseID
    FROM dbo.Topic
    WHERE (@TopicID IS NULL OR TopicID = @TopicID);
END
GO

-- Insert
CREATE OR ALTER PROCEDURE dbo.sp_Topic_Insert
    @TopicName NVARCHAR(100),
    @CourseID INT,
    @NewTopicID INT OUTPUT
AS
BEGIN
    INSERT INTO dbo.Topic (TopicName, CourseID)
    VALUES (@TopicName, @CourseID);

    SET @NewTopicID = SCOPE_IDENTITY();
END
GO

-- Update
CREATE OR ALTER PROCEDURE dbo.sp_Topic_Update
    @TopicID INT,
    @TopicName NVARCHAR(100),
    @CourseID INT
AS
BEGIN
    UPDATE dbo.Topic
    SET
        TopicName = @TopicName,
        CourseID = @CourseID
    WHERE TopicID = @TopicID;
END
GO

-- Delete
CREATE OR ALTER PROCEDURE dbo.sp_Topic_Delete
    @TopicID INT
AS
BEGIN
    DELETE FROM dbo.Topic
    WHERE TopicID = @TopicID;
END
GO

-- ---------------------------------------------------------------------------------
-- 7. QUESTION Table SPs
-- ---------------------------------------------------------------------------------

-- Select All/By ID
CREATE OR ALTER PROCEDURE dbo.sp_Question_Select
    @QuestionID INT = NULL
AS
BEGIN
    SELECT QuestionID, QuestionText, QuestionType, Points, CourseID, DifficultyLevel, IsActive
    FROM dbo.Question
    WHERE (@QuestionID IS NULL OR QuestionID = @QuestionID);
END
GO

-- Insert
CREATE OR ALTER PROCEDURE dbo.sp_Question_Insert
    @QuestionText NVARCHAR(MAX),
    @QuestionType NVARCHAR(10), -- 'MCQ' or 'TF'
    @Points INT,
    @CourseID INT,
    @DifficultyLevel NVARCHAR(50),
    @IsActive BIT,
    @NewQuestionID INT OUTPUT
AS
BEGIN
    INSERT INTO dbo.Question (QuestionText, QuestionType, Points, CourseID, DifficultyLevel, IsActive)
    VALUES (@QuestionText, @QuestionType, @Points, @CourseID, @DifficultyLevel, @IsActive);

    SET @NewQuestionID = SCOPE_IDENTITY();
END
GO

-- Update
CREATE OR ALTER PROCEDURE dbo.sp_Question_Update
    @QuestionID INT,
    @QuestionText NVARCHAR(MAX),
    @QuestionType NVARCHAR(10),
    @Points INT,
    @CourseID INT,
    @DifficultyLevel NVARCHAR(50),
    @IsActive BIT
AS
BEGIN
    UPDATE dbo.Question
    SET
        QuestionText = @QuestionText,
        QuestionType = @QuestionType,
        Points = @Points,
        CourseID = @CourseID,
        DifficultyLevel = @DifficultyLevel,
        IsActive = @IsActive
    WHERE QuestionID = @QuestionID;
END
GO

-- Delete
CREATE OR ALTER PROCEDURE dbo.sp_Question_Delete
    @QuestionID INT
AS
BEGIN
    DELETE FROM dbo.Question
    WHERE QuestionID = @QuestionID;
END
GO

-- ---------------------------------------------------------------------------------
-- 8. CHOICE Table SPs
-- ---------------------------------------------------------------------------------

-- Select All/By ID
CREATE OR ALTER PROCEDURE dbo.sp_Choice_Select
    @ChoiceID INT = NULL
AS
BEGIN
    SELECT ChoiceID, ChoiceText, QuestionID, IsCorrect
    FROM dbo.Choice
    WHERE (@ChoiceID IS NULL OR ChoiceID = @ChoiceID);
END
GO

-- Insert
CREATE OR ALTER PROCEDURE dbo.sp_Choice_Insert
    @ChoiceText NVARCHAR(MAX),
    @QuestionID INT,
    @IsCorrect BIT,
    @NewChoiceID INT OUTPUT
AS
BEGIN
    INSERT INTO dbo.Choice (ChoiceText, QuestionID, IsCorrect)
    VALUES (@ChoiceText, @QuestionID, @IsCorrect);

    SET @NewChoiceID = SCOPE_IDENTITY();
END
GO

-- Update
CREATE OR ALTER PROCEDURE dbo.sp_Choice_Update
    @ChoiceID INT,
    @ChoiceText NVARCHAR(MAX),
    @QuestionID INT,
    @IsCorrect BIT
AS
BEGIN
    UPDATE dbo.Choice
    SET
        ChoiceText = @ChoiceText,
        QuestionID = @QuestionID,
        IsCorrect = @IsCorrect
    WHERE ChoiceID = @ChoiceID;
END
GO

-- Delete
CREATE OR ALTER PROCEDURE dbo.sp_Choice_Delete
    @ChoiceID INT
AS
BEGIN
    DELETE FROM dbo.Choice
    WHERE ChoiceID = @ChoiceID;
END
GO

-- ---------------------------------------------------------------------------------
-- 9. DepartmentCourse Table SPs (Composite Key)
-- ---------------------------------------------------------------------------------

-- Select All/By Composite Key
CREATE OR ALTER PROCEDURE dbo.sp_DepartmentCourse_Select
    @DeptID INT = NULL,
    @CourseID INT = NULL
AS
BEGIN
    SELECT DeptID, CourseID, IsRequired
    FROM dbo.DepartmentCourse
    WHERE (@DeptID IS NULL OR DeptID = @DeptID)
      AND (@CourseID IS NULL OR CourseID = @CourseID);
END
GO

-- Insert
CREATE OR ALTER PROCEDURE dbo.sp_DepartmentCourse_Insert
    @DeptID INT,
    @CourseID INT,
    @IsRequired BIT
AS
BEGIN
    INSERT INTO dbo.DepartmentCourse (DeptID, CourseID, IsRequired)
    VALUES (@DeptID, @CourseID, @IsRequired);
END
GO

-- Update
CREATE OR ALTER PROCEDURE dbo.sp_DepartmentCourse_Update
    @DeptID INT,
    @CourseID INT,
    @IsRequired BIT
AS
BEGIN
    UPDATE dbo.DepartmentCourse
    SET
        IsRequired = @IsRequired
    WHERE DeptID = @DeptID AND CourseID = @CourseID;
END
GO

-- Delete
CREATE OR ALTER PROCEDURE dbo.sp_DepartmentCourse_Delete
    @DeptID INT,
    @CourseID INT
AS
BEGIN
    DELETE FROM dbo.DepartmentCourse
    WHERE DeptID = @DeptID AND CourseID = @CourseID;
END
GO

-- ---------------------------------------------------------------------------------
-- 10. InstructorCourse Table SPs (Composite Key)
-- ---------------------------------------------------------------------------------

-- Select All/By Composite Key
CREATE OR ALTER PROCEDURE dbo.sp_InstructorCourse_Select
    @InstructorID INT = NULL,
    @CourseID INT = NULL
AS
BEGIN
    SELECT InstructorID, CourseID, AssignmentDate
    FROM dbo.InstructorCourse
    WHERE (@InstructorID IS NULL OR InstructorID = @InstructorID)
      AND (@CourseID IS NULL OR CourseID = @CourseID);
END
GO

-- Insert
CREATE OR ALTER PROCEDURE dbo.sp_InstructorCourse_Insert
    @InstructorID INT,
    @CourseID INT,
    @AssignmentDate DATE
AS
BEGIN
    INSERT INTO dbo.InstructorCourse (InstructorID, CourseID, AssignmentDate)
    VALUES (@InstructorID, @CourseID, @AssignmentDate);
END
GO

-- Update
CREATE OR ALTER PROCEDURE dbo.sp_InstructorCourse_Update
    @InstructorID INT,
    @CourseID INT,
    @AssignmentDate DATE
AS
BEGIN
    UPDATE dbo.InstructorCourse
    SET
        AssignmentDate = @AssignmentDate
    WHERE InstructorID = @InstructorID AND CourseID = @CourseID;
END
GO

-- Delete
CREATE OR ALTER PROCEDURE dbo.sp_InstructorCourse_Delete
    @InstructorID INT,
    @CourseID INT
AS
BEGIN
    DELETE FROM dbo.InstructorCourse
    WHERE InstructorID = @InstructorID AND CourseID = @CourseID;
END
GO

-- =================================================================================
-- تم توليد 40 Stored Procedure لعمليات CRUD
-- =================================================================================
-- SP to Calculate Course Grade from All Exams
CREATE OR ALTER PROCEDURE sp_CalculateCourseGrade
    @StudentID INT,
    @CourseID INT
AS
BEGIN
    DECLARE @TotalScore INT, @MaxScore INT, @Percentage DECIMAL(5,2);
    
    -- Sum all exam grades for this course
    SELECT 
        @TotalScore = SUM(eg.TotalScore),
        @MaxScore = SUM(eg.MaxScore)
    FROM ExamGrade eg
    INNER JOIN Exam e ON eg.ExamID = e.ExamID
    WHERE eg.StudentID = @StudentID 
      AND e.CourseID = @CourseID;
    
    IF @MaxScore > 0
        SET @Percentage = (@TotalScore * 100.0) / @MaxScore;
    ELSE
        SET @Percentage = 0;
    
    -- Insert or Update CourseGrade
    MERGE CourseGrade AS target
    USING (SELECT @StudentID AS StudentID, @CourseID AS CourseID) AS source
    ON target.StudentID = source.StudentID AND target.CourseID = source.CourseID
    WHEN MATCHED THEN
        UPDATE SET 
            TotalScore = @TotalScore,
            MaxScore = @MaxScore,
            Percentage = @Percentage,
            LetterGrade = CASE 
                WHEN @Percentage >= 85 THEN 'A'
                WHEN @Percentage >= 75 THEN 'B'
                WHEN @Percentage >= 65 THEN 'C'
                WHEN @Percentage >= 50 THEN 'D'
                ELSE 'F'
            END,
            Status = CASE WHEN @Percentage >= 50 THEN 'Pass' ELSE 'Fail' END,
            GradeDate = GETDATE()
    WHEN NOT MATCHED THEN
        INSERT (StudentID, CourseID, TotalScore, MaxScore, Percentage, LetterGrade, Status)
        VALUES (@StudentID, @CourseID, @TotalScore, @MaxScore, @Percentage,
                CASE 
                    WHEN @Percentage >= 85 THEN 'A'
                    WHEN @Percentage >= 75 THEN 'B'
                    WHEN @Percentage >= 65 THEN 'C'
                    WHEN @Percentage >= 50 THEN 'D'
                    ELSE 'F'
                END,
                CASE WHEN @Percentage >= 50 THEN 'Pass' ELSE 'Fail' END);
END;
GO
--المشكلة: الـ SP بتاخد 10 parameters بس للأسئلة (Q1-Q10)
CREATE OR ALTER PROCEDURE dbo.SubmitExamAnswers_JSON
    @StudentID INT,
    @ExamID INT,
    @AnswersJSON NVARCHAR(MAX) -- [{"QuestionID":1,"ChoiceID":5}, ...]
AS
BEGIN
    -- Parse JSON and insert
    INSERT INTO StudentAnswer (StudentID, ExamID, QuestionID, SelectedChoiceID, QuestionPoints)
    SELECT 
        @StudentID,
        @ExamID,
        JSON_VALUE(value, '$.QuestionID'),
        JSON_VALUE(value, '$.ChoiceID'),
        q.Points
    FROM OPENJSON(@AnswersJSON) 
    CROSS APPLY (
        SELECT Points 
        FROM Question 
        WHERE QuestionID = JSON_VALUE(value, '$.QuestionID')
    ) q;
END;

