package com.demo.student_management.entity;

import com.fasterxml.jackson.annotation.JsonIgnore;
import jakarta.persistence.*;
import lombok.*;

import java.time.LocalDate;
import java.util.ArrayList;
import java.util.List;

@Entity
@Table(name = "HOCSINH")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
@EqualsAndHashCode(onlyExplicitlyIncluded = true)
public class HocSinh {

    @Id
    @Column(name = "ID_HOCSINH", length = 10, nullable = false)
    @EqualsAndHashCode.Include
    private String idHocSinh;

    @ManyToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "ID_LOP", nullable = false)
    private Lop lop;

    @Column(name = "TEN", nullable = false, length = 255)
    private String ten;

    @Column(name = "GIOI_TINH", length = 10)
    private String gioiTinh;

    @Column(name = "NGAY_SINH")
    private LocalDate ngaySinh;

    @Column(name = "DIA_CHI", length = 255)
    private String diaChi;

    @Column(name = "EMAIL", length = 255)
    private String email;

    @OneToMany(mappedBy = "hocSinh", fetch = FetchType.LAZY)
    @JsonIgnore
    @Builder.Default
    private List<ChiTietDiem> chiTietDiems = new ArrayList<>();
}