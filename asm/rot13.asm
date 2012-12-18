; rot13.asm
; by Joseph Sung, 5/9/2001

; Originally from IBM PC / MASM 80x86, ISBN 957-723-204-3
; CH2-3.ASM
; Read strings from keyboard then output to screen

; Modified to fit UCSC CMPE12C Spring 2001 Asmt 7, Rot 13 encoding

        .MODEL SMALL
        .STACK 200
        .DATA

; declare a buffer here for holding strings (100 bytes)
buffer  db      41, 100 dup (0)

        .CODE

main:
        .startup
;Read a string from keyboard
        call    GETSTRING       ; calling a GETSTRING procedure in 80x86
;Output a Carriage Return and  Line Feed
        call    OUTCRLF
;Get buffer address
        lea     bx,buffer       ; equivalent to ldx/ldy to load address of string
        add     bx, 2
;The following program section used for printing the string
;(Print char by char until encounter ENTER -- 0dh)
r13_out_lp:
        mov     ah,02h
        mov     dl,byte ptr ds:[bx]     ; byte pointer based address calculation/dereferencing
              ; dl = byte content of absolute address represented by ds:[bs]
        cmp     dl,0dh          ; compare if it's equal to CR
        je      done            ; if equal jump to done label

        ; if not equal, we want to print that char out to screen in rot13.
        ; and we do rot13 process starting from here.

        cmp     dl, 'a' ; compare with 'a'
        ; unsigned jump condition  Below (B), and Above (A)
        ; similar to HC11's  BLS, BLO, BHS, BHI, (LOwer or HIgher)
        jb      UPC_CHK ; if below, jump to UPC_CHK skip because 'A' < 'a'
                        ; in ASCII
        cmp     dl, 'z'
        ja      skip
        jmp     LC_enc

UPC_CHK:
        cmp     dl, 'A'
        jb      skip    ; skip because we are print 'a'-'z' or 'A'-'Z'
        cmp     dl, 'Z'
        ja      skip    ; skip because we are print 'a'-'z' or 'A'-'Z'
; UP_enc immediately coded after UPC_CHK fall through (is_UPPERCASE)        
        PUSH    CX
        mov     cl, 'A'
        add     cl, 13
        cmp     cl, dl
        ja      PLUS_enc
        POP     CX
        jmp    SUB_enc

LC_enc:
        PUSH    CX
        mov     cl, 'a'
        add     cl, 13
        cmp     cl, dl
        ja      PLUS_enc
        POP     CX

SUB_enc:
        sub     dl, 13  ; rot13 subtract encoding for this char
        jmp     skip    ; not skipping, we are only connecting to
                        ; regular printing
PLUS_enc:
        add     dl, 13  ; rot13 plus encoding for this char
                        ; no need to jmp skip here, code will fall through
                        ; to next line, that's where to go after +13

skip:

        int     21h             ; execute user interrupt,
                                ; will execute 02h char print
        inc     bx              ; increment bx register for next char
        jmp     r13_out_lp

; Procedure: GETSTRING, using 80x86 building 0Ah interrupt call to
;                       get string inputs from keyboard
; Read a string from keyboard
; string returned in variable name: buffer
GETSTRING       PROC    NEAR    ; 80x86 Procedure declaration
        mov     ah,0ah  ; System parameter in AH from reading
                        ; in strings: AH=0Ah
        mov     dx,offset buffer
        int     21h
        ret
GETSTRING       ENDP    ; 80x86 Procedure declaration end

OUTCRLF         PROC    NEAR
;Output a Carriage Return
        mov     ah,02h
        mov     dl,0dh
        int     21h
;Output a Line Feed
        mov     ah,02h
        mov     dl,0ah
        int     21h
        ret
OUTCRLF         ENDP

done:
        .exit
        END

