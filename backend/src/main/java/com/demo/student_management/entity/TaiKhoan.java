package com.demo.student_management.entity;

import com.fasterxml.jackson.annotation.JsonIgnore;
import jakarta.persistence.*;
import lombok.*;

import java.time.LocalDate;

@Entity
@Table(name = "TAIKHOAN")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
@EqualsAndHashCode(onlyExplicitlyIncluded = true)
public class TaiKhoan {

    @Id
    @Column(name = "ID_TAIKHOAN", columnDefinition = "char(36)", nullable = false)
    @EqualsAndHashCode.Include
    private String idTaiKhoan;

    @Column(name = "HASH", length = 255)
    private String hash;

    @Column(name = "TENDANGNHAP", length = 255, nullable = false, unique = true)
    private String tenDangNhap;

    @Column(name = "MATKHAU", length = 255, nullable = false)
    private String matKhau;

    @Column(name = "LOAI_TAIKHOAN", length = 50)
    private String loaiTaiKhoan;

    @Column(name = "TEN", length = 255)
    private String ten;

    @Column(name = "CMND", length = 13)
    private String cmnd;

    @Column(name = "NGAY_SINH")
    private LocalDate ngaySinh;

    @Column(name = "GIOI_TINH", length = 10)
    private String gioiTinh;

    @Column(name = "SDT", length = 11)
    private String sdt;

    @Column(name = "EMAIL", length = 255)
    private String email;

    @Column(name = "ACTIVE")
    @Builder.Default
    private Boolean active = true;

    @OneToOne(mappedBy = "taiKhoan", fetch = FetchType.LAZY)
    @JsonIgnore
    private GiaoVien giaoVien;

    @OneToOne(mappedBy = "taiKhoan", fetch = FetchType.LAZY)
    @JsonIgnore
    private BanQuanLy banQuanLy;
}